using System.Xml.Linq;
using APG.MessageQueue.Contracts.Transactions;
using APG.MessageQueue.Interfaces;
using APG.MessageQueue.Mpcss.ActiveMQTransport;
using APG.MessageQueue.Mpcss.Interfaces;
using APGDigitalIntegration.Common.CommonMethods;
using APGDigitalIntegration.Common.CommonMethods.MessageBuilders;
using APGDigitalIntegration.Common.CommonServices;
using APGDigitalIntegration.Common.CommonViewModels.Common;
using APGDigitalIntegration.Common.CommonViewModels.Payment_New;
using APGDigitalIntegration.Common.CommonViewModels.Payment_New.PaymentMesssages;
using APGDigitalIntegration.Common.CommonViewModels.Payment_New.SupplementaryData;
using APGDigitalIntegration.Common.CommonViewModels.Payment.Enquiry;
using APGDigitalIntegration.Common.CommonViewModels.Response;
using APGDigitalIntegration.Constant;
using APGDigitalIntegration.Domain.Interfaces;
using APGDigitalIntegration.Domain.Models;
using APGDigitalIntegration.DomainHelper.ExtensionMethods;
using APGDigitalIntegration.DomainHelper.Filters;
using APGDigitalIntegration.IAL.External.Mpcss.Interfaces;
using APGDigitalIntegration.IAL.External.Mpcss.Interfaces.ICBOHostAdapters.Transactional;
using APGDigitalIntegration.IAL.Internal.Interfaces.APGTransaction;
using APGDigitalIntegration.IAL.Internal.ViewModels.APGTransaction;
using APGExecutions.IAL.Internal.Interfaces.APGFundamentals;
using APGFundamentals.Application.Helper;
using APGMPCSSIntegration.Common.CommomMethods.MessageBuilders;
using APGMPCSSIntegration.Common.CommonViewModels.Request;
using APGMPCSSIntegration.Constant;
using Envelope = APGDigitalIntegration.Common.CommonViewModels.Common.Envelope;

namespace APGDigitalIntegration.IAL.External.Mpcss.Hosts.CBOHostAdapters.Transactional;

public class PaymentEnquiryHostAdapter : IPaymentEnquiryHostAdapter
{
    #region Fields

    private readonly IMpcssCommunicator _mpcssCommunicator;
    private readonly IConfParamHelperService _confParamHelperService;
    private readonly ResponseMessageHandler _messageHandler;
    private readonly IDigitalTransactionRepository _digitalTransactionRepository;
    private readonly IMerchantMPCSSTransactionRequestsRepository _merchantMPCSSTransactionRequestsRepository;
    private readonly ITransactionApiService _transactionApiService;
    private readonly IMessageQueue _messageQueue;
    private readonly IMPCSSCommunicationLogService _communicationLogService;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IMPCSSMessageBuilder _mpcssMessageBuilder;

    #endregion

    #region Constructor 
    public PaymentEnquiryHostAdapter(IMpcssCommunicator mpcssCommunicator, 
        IConfParamHelperService confParamHelperService,
        ResponseMessageHandler messageHandler, 
        IMessageQueue messageQueue,
        IMPCSSCommunicationLogService communicationLogService,
        IDateTimeProvider dateTimeProvider,
        IMPCSSMessageBuilder mpcssMessageBuilder, 
        IDigitalTransactionRepository digitalTransactionRepository,
        IMerchantMPCSSTransactionRequestsRepository merchantMPCSSTransactionRequestsRepository,
        ITransactionApiService transactionApiService)
    {
        _mpcssCommunicator = mpcssCommunicator;
        _messageHandler = messageHandler;
        _messageQueue = messageQueue;
        _communicationLogService = communicationLogService;
        _dateTimeProvider = dateTimeProvider;
        _mpcssMessageBuilder = mpcssMessageBuilder;
        _digitalTransactionRepository = digitalTransactionRepository;
        _merchantMPCSSTransactionRequestsRepository = merchantMPCSSTransactionRequestsRepository;
        _transactionApiService = transactionApiService;
        _confParamHelperService = confParamHelperService;
    }

    #endregion

    #region Public Methods

    public async Task<ServiceResponse> Execute(PaymentEnquiryRequest paymentEnquiryRequest)
    {
        var originalTransactionFilter = new  DigitalTransactionFilter()
        {
            TransactionIdentifier = paymentEnquiryRequest.DigitalTransactionIdentifierType,
            IdentifierValue = paymentEnquiryRequest.TransactionIdentifierValue
        };
        
        var originalDigitalTransaction = await _digitalTransactionRepository.GetTransaction(originalTransactionFilter);
        var originalTransaction = await _transactionApiService.GetTransactionById(originalDigitalTransaction.Id);
        
        var enquiryTransaction = await AddDigitalTransaction(paymentEnquiryRequest, originalDigitalTransaction, originalTransaction);
            
        var messageRequesitesDto = MpcssMethods.PopulateMessageType(MPCSSRecordRequest.PaymentEnquiry);

        var externalRequest = await ConvertToExternalRequest(enquiryTransaction.ExternalTransactionId, originalDigitalTransaction);
        this._communicationLogService.SetRequestDatetime(DateTime.Now);
        this._communicationLogService.SetTransactionTypeId(TransactionType.WalletEnquiry);
        this._communicationLogService.SetExternalRequest(externalRequest);
        await this._communicationLogService.SetExternalRequestTime();
        this._communicationLogService.MPCSSCommunicationLogModel.OriginalExternalMsgId = originalDigitalTransaction.ExternalTransactionId;
          _communicationLogService.MPCSSCommunicationLogModel.BankId = paymentEnquiryRequest.BankId;

        this._communicationLogService.SetMsgId(enquiryTransaction.ExternalTransactionId);

        var externalResponse = await SendMessage(externalRequest, messageRequesitesDto.QueueType).ConfigureAwait(false);

        return new ServiceResponse(
            success: true,
            responseCode: ResponseCodes.Success,
            message: _messageHandler.GetMessage(PaymentSuccessMessage.RequestSentSuccessfully));
    }

    #endregion

    #region Private Methods
        
    private async Task<DigitalTransaction> AddDigitalTransaction(PaymentEnquiryRequest paymentEnquiryRequest,
        DigitalTransaction originalDigitalTransaction, TransactionViewModel originalTransaction)
    {
        var participantPrefix = await _confParamHelperService.GetValue<string>(ConfigParam.MPCSSPSPRouteCode);

        var now = await _dateTimeProvider.NowByBankId(paymentEnquiryRequest.BankId);
        var request = await _merchantMPCSSTransactionRequestsRepository.Add(new MerchantMPCSSTransactionRequest()
        {
            QROrderId = null,
            UniqueNotificationId = paymentEnquiryRequest.RequestSource == RequestSources.System ? "SYSTEM" : paymentEnquiryRequest.UniqueNotificationId,
            Status = MPCSSStatus.Initiated.ToString(),
            TransactionType = MPCSSRecordRequest.PaymentEnquiry.ToString(),
            ParticipantPrefix = participantPrefix,
            RequestSourceId = (int)paymentEnquiryRequest.RequestSource,
            CreationDate = now,
            Language = Thread.CurrentThread.CurrentCulture.ToString()
        });
        await _merchantMPCSSTransactionRequestsRepository.UnitOfWork.Commit();
        var msgIdentificationCode = request.MessageId;

        var mpcssTimeout = await _confParamHelperService.GetValue<short>(ConfigParam.MPCSSOutwardTransactionInternalTimeout);
        var trx = new DigitalTransaction()
        {
            Amount = originalDigitalTransaction.Amount,
            Status = TransactionStatus.Processing,
            CurrencyId = originalDigitalTransaction.CurrencyId,
            TransactionTypeId = (int)TransactionType.WalletEnquiry,
            TerminalNodeId = paymentEnquiryRequest.TerminalNodeId,
            MerchantRefId = paymentEnquiryRequest.MerchantRefId,
            Id = paymentEnquiryRequest.TransactionId,
            BankId = paymentEnquiryRequest.BankId,
            CreatedDatetime = now,
            MaxResponseDatetime = now.AddSeconds(mpcssTimeout),
            IsRefunded = false,
            ExternalTransactionId = msgIdentificationCode,
            TransactionMethodId = (int)TransactionMethods.WalletECommerce,
            OrderId = Guid.Empty,
            RequestSourceId = (int)paymentEnquiryRequest.RequestSource,
            AggregatorId = paymentEnquiryRequest.AggregatorId,
            MerchantAccountTypeId = paymentEnquiryRequest.SettAccType,
            OriginalExternalTransactionId = originalDigitalTransaction.ExternalTransactionId,
            OriginalDigitalTransactionIdN = originalDigitalTransaction.IdN,
            OriginalTransactionIdN = originalTransaction.IdN
        };
            
        _digitalTransactionRepository.Add(trx);
        await _digitalTransactionRepository.UnitOfWork.Commit();
        
        return trx;
    }

    private async Task<Envelope> ConvertToExternalRequest(string msgIdentificationCode, DigitalTransaction originalTransaction)
    {
        var originalMessageName = MpcssMessageConstants.GetMPCSSMessageTypeByTransactionType((TransactionType)originalTransaction.TransactionTypeId);
        
        var now = await _dateTimeProvider.SystemNow();
        var externalRequest = new MPCSSPaymentEnquiryExternalRequest
        {
            GroupHeader = new PaymentEnquiryGroupHeader
            {
                MessageIdentification = msgIdentificationCode,
                CreatedDateTime = now.ToISODateTime()
            },
            EnquiryOriginalGroupInformation = new EnquiryOriginalGroupInformation()
            {
                OriginalMessageIdentification = originalTransaction.ExternalTransactionId,
                OriginalMessageNameIdentification = originalMessageName,
                OriginalCreationDateAndTime = originalTransaction.CreatedDatetime.ToISODateTime()
            }
        };


        return _mpcssMessageBuilder.ConvertToExternalRequest(externalRequest, externalRequest.GroupHeader.CreatedDateTime, msgIdentificationCode, MPCSSMessageTypes.PAYMENT_ENQUIRY_MESSAGE_TYPE, true);
    }
        
    private async Task<ServiceResponse> SendMessage(Envelope message, string queue)
    {
        // Read this key from ConfParams.
        var isSimulated = await _confParamHelperService.GetValue<string>(ConfigParam.SimulateMPCSSTransaction, null).ConfigureAwait(false);

        if (!string.IsNullOrEmpty(isSimulated) && isSimulated == "true") 
            return await ConstructSimulatedResponse(message);
            
        await _mpcssCommunicator.SendMessage(message, queue, ActiveMQMessageTypes.Text);
        return new ServiceResponse(
            success: true,
            responseCode: ResponseCodes.Success,
            message: _messageHandler.GetMessage(PaymentSuccessMessage.RequestSentSuccessfully));
    }

    private async Task<ServiceResponse> ConstructSimulatedResponse(Envelope request)
    {
        var isSimulatedWalletEnquirySuccess = await _confParamHelperService.GetValue<bool>(ConfigParam.IsSimulatedWalletEnquirySuccess);

        #region Fields

        string groupStatus;
        StatusReasonInformation statusInformationResponseDto = null;
        
        var originalTransactionId = CommonMethods.GetXMLAttributeValue(XElement.Parse(Convert.ToString(request.Content.Value)), "MsgId");

        #endregion

        #region Error Mapping
            
        if (isSimulatedWalletEnquirySuccess == false)
        {
            groupStatus = "RJCT";
            statusInformationResponseDto = new StatusReasonInformation
            {
                Reason = new Reason
                {
                    OriginalGroupStatusProprietary = "1130"
                },
                AdditionalInformation = "Payment was not found"
            };
        }
        else
        {
            groupStatus = "ACSP";
        }

        #endregion

        #region XML Construction
            
        var now = await _dateTimeProvider.SystemNow();
        var response = new MPCSSPaymentStatusReport()
        {
            GroupHeader = new GroupHeader()
            {
                MsgId = "850s3pmlmy3j5jq2",
                CreatedDateTime = now.ToISODateTime(),
                InstructingAgent = new InstructingAgent()
                {
                    FinancialInstitutionIdentification = new FinancialInstitutionIdentification()
                    {
                        BICFI = "TESTOMR3"
                    }
                },
                InstructedAgent = new InstructedAgent()
                {
                    FinancialInstitutionIdentification = new FinancialInstitutionIdentification()
                    {
                        BICFI = "AMPLOMRU"
                    }
                },
                TotalInterbankSettlementAmount= new TotalInterBankSettlementAmount()
                {
                    Value = "10",
                    Currency = "OMR"
                }
            },
            OrgnlGrpInfAndSts = new OriginalGroupStatusAndInformation()
            {
                OriginalMessageId = originalTransactionId,
                OriginalMessageStatus = MPCSSMessageTypes.PAYMENT_ENQUIRY_MESSAGE_TYPE,
                GroupStatus = groupStatus,
                StsRsnInf = statusInformationResponseDto
            },
            SplmtryData = new SupplementaryData()
            {
                PlcAndNm = "ACHSupplementaryData",
                Envlp = new Envlp()
                {
                    achSupplementaryData = new AchSupplementaryData()
                    {
                        BatchSource = "20",
                        SessionSequence = "305",
                        // GroupMerchantId = groupMerchantId,
                        // TerminalId = terminalId
                    }
                }
            }
        };

        var paymentReportResponseDto = new MPCSSPaymentStatusReportRoot()
        {
            MPCSSPaymentStatusReport = response
        };

        var datetime = response.GroupHeader.CreatedDateTime;

        var message = _mpcssMessageBuilder.ConvertToExternalRequest(paymentReportResponseDto, datetime, response.OrgnlGrpInfAndSts.OriginalMessageId,
            MPCSSMessageTypes.PAYMENT_ENQUIRY_MESSAGE_TYPE, true);
        
        #endregion

        #region Active Mq for Receiver
            
        Task.Run(async () =>
        {
            await Task.Delay(2000);
            await _mpcssCommunicator.SendMessage(message, MPCSSQueues.PaymentEnquiryResponseQueue, ActiveMQMessageTypes.Text);
        });
            
        #endregion

        return new ServiceResponse(success: true, responseCode: ResponseCodes.Success, message: _messageHandler.GetMessage(PaymentSuccessMessage.RequestSentSuccessfully));
    }


    #endregion
}