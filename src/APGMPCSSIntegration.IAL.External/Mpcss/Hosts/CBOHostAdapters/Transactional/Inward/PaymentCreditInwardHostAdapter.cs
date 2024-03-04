using APG.MessageQueue.Interfaces;
using APG.MessageQueue.Mpcss.ActiveMQTransport;
using APG.MessageQueue.Mpcss.Interfaces;
using APG.MessageQueue.Interfaces;
using APGDigitalIntegration.Common.CommonMethods.MessageBuilders;
using APGDigitalIntegration.Common.CommonServices;
using APGDigitalIntegration.Common.CommonViewModels.Common;
using APGDigitalIntegration.Common.CommonViewModels.Payment_New;
using APGDigitalIntegration.Common.CommonViewModels.Payment_New.PaymentMesssages;
using APGDigitalIntegration.Common.CommonViewModels.Payment_New.SupplementaryData;
using APGDigitalIntegration.Common.CommonViewModels.Request;
using APGDigitalIntegration.Common.CommonViewModels.Response;
using APGDigitalIntegration.Constant;
using APGDigitalIntegration.Domain.Interfaces;
using APGDigitalIntegration.Domain.Models;
using APGDigitalIntegration.DomainHelper.ExtensionMethods;
using APGDigitalIntegration.IAL.External.Mpcss.Communicators;
using APGDigitalIntegration.IAL.External.Mpcss.Interfaces;
using APGDigitalIntegration.IAL.External.Mpcss.Interfaces.ICBOHostAdapters.Transactional.Inward;
using APGDigitalIntegration.IAL.Internal.Interfaces.APGLog;
using APGDigitalIntegration.IAL.Internal.Interfaces.APGTransaction;
using APGDigitalIntegration.IAL.Internal.Services.APGFundamental;
using APGDigitalIntegration.IAL.Internal.ViewModels.APGTransaction;
using APGExecutions.IAL.Internal.Interfaces.APGFundamentals;
using APGMPCSSIntegration.Common.CommomMethods.MessageBuilders;
using APGMPCSSIntegration.Common.CommonViewModels.Response;
using APGMPCSSIntegration.Constant;
using APGMPCSSIntegration.DomainHelper;
using APGMPCSSIntegration.DomainHelper.Exceptions;
using Envelope = APGDigitalIntegration.Common.CommonViewModels.Common.Envelope;
using TransactionStatus = APGDigitalIntegration.Common.CommonViewModels.Request.TransactionStatus;
using APGFundamentals.Application.Helper;
using System.Globalization;

namespace APGDigitalIntegration.IAL.External.Mpcss.Hosts.CBOHostAdapters.Transactional.Inward
{
    public class PaymentCreditInwardHostAdapter : IPaymentCreditInwardHostAdapter
    {
        #region Fields

        private readonly IMpcssCommunicator _mpcssCommunicator;
        private readonly IConfParamHelperService _confParamHelperService;
        private readonly ResponseMessageHandler _messageHandler;
        private readonly IMPCSSCommunicationLogService _mpcssCommunicationLogService;
        private readonly ILoggingService _loggingService;
        private readonly IBankApiService _bankApiService;
        private readonly IMPCSSMessageBuilder _mpcssMessageBuilder;
        private readonly IMerchantMPCSSTransactionRequestsRepository _mpcssTransactionRequestsRepository;
        private readonly IDateTimeProvider _dateTimeProvider;

        #endregion

        #region Constructor

        public PaymentCreditInwardHostAdapter(IMpcssCommunicator mpcssCommunicator,
            IConfParamHelperService confParamHelperService,
            ResponseMessageHandler messageHandler,
            IMPCSSCommunicationLogService mpcssCommunicationLogService,
            ILoggingService loggingService,
            IBankApiService bankApiService,
            IMPCSSMessageBuilder mpcssMessageBuilder,
            IMerchantMPCSSTransactionRequestsRepository mpcssTransactionRequestsRepository,
            IDateTimeProvider dateTimeProvider)
        {
            _mpcssCommunicator = mpcssCommunicator;
            _messageHandler = messageHandler;
            _mpcssCommunicationLogService = mpcssCommunicationLogService;
            _loggingService = loggingService;
            _bankApiService = bankApiService;
            _mpcssMessageBuilder = mpcssMessageBuilder;
            _mpcssTransactionRequestsRepository = mpcssTransactionRequestsRepository;
            _confParamHelperService = confParamHelperService;
            _dateTimeProvider=dateTimeProvider;
        }

        #endregion

        #region Public Methods

        public async Task<BaseInternalResponse> Execute(CreditPaymentInternalRequest internalCreditPaymentInternalRequest, TransactionStatus transactionStatus, CancellationToken cancellationToken)
        {
            // External Response
            // Internal Response
            BaseInternalResponse baseInternalResponse = default;
            Envelope externalResponse = default;
            try
            {
                if (internalCreditPaymentInternalRequest is not MPCSSInwardCreditPaymentInternalRequest mpcssRequest)
                    throw new InvalidOperationException();

                // await Task.Delay(25000);
                _mpcssCommunicationLogService.SetTransactionTypeId(TransactionType.P2BPush);

                var participantPrefix = await _confParamHelperService.GetValue<string>(ConfigParam.MPCSSPSPRouteCode);

                int? errorCode = null;
                if (!string.IsNullOrWhiteSpace(transactionStatus.ResponseCode) &&
                    int.TryParse(transactionStatus.ResponseCode, out int tempErrorCode))
                {
                    errorCode = tempErrorCode;
                }

                var now = await _dateTimeProvider.NowByBankId(internalCreditPaymentInternalRequest.BankId);

                var request = await _mpcssTransactionRequestsRepository.Add(new MerchantMPCSSTransactionRequest
                {
                    QROrderId = mpcssRequest.WalletOrderId,
                    UniqueNotificationId = mpcssRequest.UniqueIdentificationId,
                    Status = transactionStatus.IsSuccess ? MpcssMessageConstants.ResponseStatus.Accepted : MpcssMessageConstants.ResponseStatus.Rejected,
                    TransactionType = MPCSSRecordRequest.PaymentInwardCreditRequest.ToString(),
                    ErrorCode = errorCode,
                    ErrorMessage = transactionStatus.ErrorList.FirstOrDefault(),
                    ParticipantPrefix = participantPrefix,
                    RequestSourceId = (int)internalCreditPaymentInternalRequest.RequestSource,
                    CreationDate=now,
                    Language = Thread.CurrentThread.CurrentCulture.ToString(),
                });

                await _mpcssTransactionRequestsRepository.UnitOfWork.Commit();
                var msgIdentificationCode = request.MessageId;
                var mpcssMessage = MpcssMethods.PopulateMessageType(MPCSSRecordRequest.PaymentStatusReport);

                var bankConfigurations = await _bankApiService.GetBank(internalCreditPaymentInternalRequest.BankId);
                var instructedAgentBICFI = bankConfigurations.Data?.BankConfiguration?.BankIdentifierCode;
                if (string.IsNullOrWhiteSpace(instructedAgentBICFI))
                    throw new BusinessException("Instructed Agent [Bank] BICFI Not found", "51");

                externalResponse = ConvertToExternalResponse(mpcssRequest, mpcssMessage, transactionStatus, instructedAgentBICFI, msgIdentificationCode);


                _mpcssCommunicationLogService.MPCSSCommunicationLogModel.OriginalExternalMsgId = mpcssRequest.OriginalMessageId;
                _mpcssCommunicationLogService.SetRequestDatetime(DateTime.Now);
                _mpcssCommunicationLogService.MPCSSCommunicationLogModel.BankId = internalCreditPaymentInternalRequest.BankId;
                await _mpcssCommunicationLogService.SetExternalResponseTime();
                await _mpcssCommunicationLogService.SetInternalResponseTime();

                if (cancellationToken.IsCancellationRequested)
                {
                    baseInternalResponse = BaseInternalResponse.GetTimeoutResponse(mpcssRequest.OriginalMessageId);
                    return baseInternalResponse;
                }

                _mpcssCommunicationLogService.SetExternalResponse(externalResponse);
                _mpcssCommunicationLogService.SetMsgId(msgIdentificationCode);

                await SendMessage(externalResponse, mpcssMessage.QueueType).ConfigureAwait(false);

                var hostData = new Dictionary<string, string>()
                {
                    { "ExternalTransactionId", msgIdentificationCode },
                    { "OriginalExternalTransactionId", mpcssRequest.OriginalMessageId },
                    { "Status", transactionStatus.IsSuccess ? MpcssMessageConstants.ResponseStatus.Accepted : MpcssMessageConstants.ResponseStatus.Rejected }
                };

                baseInternalResponse = new BaseInternalResponse
                {
                    ResponseCode = transactionStatus.ResponseCode,
                    IsSuccess = transactionStatus.IsSuccess,
                    ResponseMessage = transactionStatus.Message,
                    ExternalMessageId = msgIdentificationCode,
                    OriginalExternalMessageId = mpcssRequest.OriginalMessageId,
                    HostData = hostData
                };

                _mpcssCommunicationLogService.SetExternalRequest(externalResponse);
            }
            catch (BusinessException ex)
            {
                var exceptionId = await _loggingService.HandleException(ex);
                _mpcssCommunicationLogService.SetExceptionId(exceptionId.ToString());

                baseInternalResponse = new BaseInternalResponse
                {
                    ResponseCode = ex.ResponseCode,
                    IsSuccess = false,
                    ResponseMessage = ResponseMessages.ResponseFailure,
                    OriginalExternalMessageId = internalCreditPaymentInternalRequest.OriginalMessageId,
                    HostData = new Dictionary<string, string>(),
                    ExternalMessageId = string.Empty
                };
            }
            catch (Exception ex)
            {
                var exceptionId = await _loggingService.HandleException(ex);
                _mpcssCommunicationLogService.SetExceptionId(exceptionId.ToString());

                baseInternalResponse = new BaseInternalResponse
                {
                    ResponseCode = ResponseCodes.TechnicalException,
                    IsSuccess = false,
                    ResponseMessage = ResponseMessages.ResponseFailure,
                    OriginalExternalMessageId = internalCreditPaymentInternalRequest.OriginalMessageId,
                    HostData = new Dictionary<string, string>(),
                    ExternalMessageId = string.Empty
                };
            }
            finally
            {
                _mpcssCommunicationLogService.SetExternalResponse(externalResponse);
                _mpcssCommunicationLogService.SetInternalResponse(baseInternalResponse);
                _mpcssCommunicationLogService.MPCSSCommunicationLogModel.ResponseCode = baseInternalResponse?.ResponseCode ?? ResponseCodes.TechnicalException;
                await _mpcssCommunicationLogService.SetInternalResponseTime();
            }

            return baseInternalResponse;
        }
        #endregion

        #region Private Methods
        public Envelope ConvertToExternalResponse(MPCSSInwardCreditPaymentInternalRequest mpcssInwardCreditInternalRequest, MessageRequesitesDto messageRequesitesDto, TransactionStatus transactionStatus, string instructedAgentBICFI, string msgIdentificationCode)
        {
            var GrpHdr = new GroupHeader()
            {
                MsgId = msgIdentificationCode,
                CreatedDateTime = DateTime.UtcNow.ToISODateTime()
            };

            var instgAgt = new InstructingAgent();
            var instructingFinInstnId = new FinancialInstitutionIdentification()
            {
                BICFI = mpcssInwardCreditInternalRequest.InstructingAgentBICFI
            };
            instgAgt.FinancialInstitutionIdentification = instructingFinInstnId;
            GrpHdr.InstructingAgent = instgAgt;

            var InstdAgt = new InstructedAgent();
            var InstructedFinInstnId = new FinancialInstitutionIdentification()
            {
                BICFI = instructedAgentBICFI
            };
            InstdAgt.FinancialInstitutionIdentification = InstructedFinInstnId;
            GrpHdr.InstructedAgent = InstdAgt;

            var orgnlGrpInfAndSts = new OriginalGroupStatusAndInformation
            {
                OriginalMessageId = mpcssInwardCreditInternalRequest.OriginalMessageId,
                OriginalMessageStatus = MPCSSMessageTypes.CREDIT_MESSAGE_TYPE,
                GroupStatus = transactionStatus.IsSuccess ? MpcssMessageConstants.ResponseStatus.Accepted : MpcssMessageConstants.ResponseStatus.Rejected
            };
            var StsRsnInf = new StatusReasonInformation();
            var Rsn = new Reason
            {
                OriginalGroupStatusProprietary = transactionStatus.Message
            };
            StsRsnInf.Reason = Rsn;
            orgnlGrpInfAndSts.StsRsnInf = StsRsnInf;

            var SplmtryData = new SupplementaryData();
            SplmtryData.PlcAndNm = "ACHSupplementaryData";
            var Envlp = new Envlp();
            var achSupplementaryData = new AchSupplementaryData
            {
                //achSupplementaryData.receiverName = request.SupplementaryReceiverName;
                //achSupplementaryData.receiverIdValue = request.ReceiverIdValue;
                //achSupplementaryData.receiverIdType = request.ReceiverIdType;
                //achSupplementaryData.receiverIdIssuingCountry = request.ReceiverIdIssuingCountry;
                SessionSequence = mpcssInwardCreditInternalRequest.OriginalSessionSequence,
                BatchSource = "20"
            };
            Envlp.achSupplementaryData = achSupplementaryData;
            SplmtryData.Envlp = Envlp;

            var paymentResponse = new MPCSSPaymentStatusReport()
            {
                GroupHeader = GrpHdr,
                OrgnlGrpInfAndSts = orgnlGrpInfAndSts,
                SplmtryData = SplmtryData
            };

            var paymentReportResponseDto = new MPCSSPaymentStatusReportRoot()
            {
                MPCSSPaymentStatusReport = paymentResponse
            };

            var isSimulated = _confParamHelperService.GetValue<string>(ConfigParam.SimulateMPCSSTransaction, null).Result;
            // Simulated Environment
            if (!string.IsNullOrEmpty(isSimulated) && isSimulated == "true")
                ConstructSimulatedResponse(mpcssInwardCreditInternalRequest.Amount.ToString(), ref paymentReportResponseDto);

            var message = _mpcssMessageBuilder.ConvertToExternalRequest(paymentReportResponseDto, GrpHdr.CreatedDateTime, GrpHdr.MsgId, messageRequesitesDto.MessageType, true);

            return message;

        }
        private async Task SendMessage(Envelope message, string queue)
        {
            await _mpcssCommunicationLogService.SetExternalResponseTime();
            // Read this key from ConfParams.
            var isSimulated = await _confParamHelperService.GetValue<string>(ConfigParam.SimulateMPCSSTransaction).ConfigureAwait(false);

            if (string.IsNullOrEmpty(isSimulated) || isSimulated != "true")
                await _mpcssCommunicator.SendMessage(message, queue, ActiveMQMessageTypes.Text);
        }
        private void ConstructSimulatedResponse(string requestAmount, ref MPCSSPaymentStatusReportRoot response)
        {
            if (requestAmount.Equals("9001"))
            {
                response.MPCSSPaymentStatusReport.OrgnlGrpInfAndSts.GroupStatus = "RJCT";

                response.MPCSSPaymentStatusReport.OrgnlGrpInfAndSts.StsRsnInf = new StatusReasonInformation()
                {
                    Reason = new Reason()
                    {
                        OriginalGroupStatusProprietary = "1001"
                    },
                    AdditionalInformation = "Exceeds the balance limit"
                };
            }
            else if (requestAmount.Equals("9002"))
            {
                response.MPCSSPaymentStatusReport.OrgnlGrpInfAndSts.GroupStatus = "RJCT";

                response.MPCSSPaymentStatusReport.OrgnlGrpInfAndSts.StsRsnInf = new StatusReasonInformation()
                {
                    Reason = new Reason()
                    {
                        OriginalGroupStatusProprietary = "1001"
                    },
                    AdditionalInformation = "Reply timeout reached"
                };
            }
            else if (requestAmount.Equals("9011"))
            {
                response.MPCSSPaymentStatusReport.OrgnlGrpInfAndSts.GroupStatus = "RJCT";
                response.MPCSSPaymentStatusReport.OrgnlGrpInfAndSts.StsRsnInf = new StatusReasonInformation()
                {
                    Reason = new Reason()
                    {
                        OriginalGroupStatusProprietary = "1"
                    },
                    AdditionalInformation = "Account is closed/blocked"
                };
            }
        }
        #endregion
    }
}
