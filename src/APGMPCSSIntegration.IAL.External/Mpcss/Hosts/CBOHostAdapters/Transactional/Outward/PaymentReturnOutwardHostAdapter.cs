using System.Xml.Linq;
using APG.MessageQueue.Interfaces;
using APG.MessageQueue.Mpcss.ActiveMQTransport;
using APG.MessageQueue.Mpcss.Interfaces;
using APGDigitalIntegration.Common.CommonMethods;
using APGDigitalIntegration.Common.CommonServices;
using APGDigitalIntegration.Common.CommonViewModels.Common;
using APGDigitalIntegration.Common.CommonViewModels.Payment_New;
using APGDigitalIntegration.Common.CommonViewModels.Payment_New.PaymentMesssages;
using APGDigitalIntegration.Common.CommonViewModels.Payment_New.SupplementaryData;
using APGDigitalIntegration.Common.CommonViewModels.Response;
using APGDigitalIntegration.Constant;
using APGDigitalIntegration.Domain.Interfaces;
using APGDigitalIntegration.Domain.Models;
using APGDigitalIntegration.DomainHelper.ExtensionMethods;
using APGDigitalIntegration.DomainHelper.Filters;
using APGDigitalIntegration.DomainHelper.Interfaces;
using APGDigitalIntegration.IAL.External.Mpcss.Interfaces;
using APGDigitalIntegration.IAL.External.Mpcss.Interfaces.ICBOHostAdapters.Transactional.Outward;
using APGDigitalIntegration.IAL.Internal.Interfaces.APGTransaction;
using APGDigitalIntegration.IAL.Internal.Services.APGFundamental;
using APGDigitalIntegration.IAL.Internal.ViewModels.APGTransaction;
using APGExecutions.IAL.Internal.Interfaces.APGFundamentals;
using APGFundamentals.Application.Helper;
using APGMPCSSIntegration.Common.CommomMethods.MessageBuilders;
using APGMPCSSIntegration.Common.CommonViewModels.Request;
using APGMPCSSIntegration.Constant;
using APGMPCSSIntegration.DomainHelper.Exceptions;
using Envelope = APGDigitalIntegration.Common.CommonViewModels.Common.Envelope;
using FinancialInstitutionIdentification = APGDigitalIntegration.Common.CommonViewModels.Payment_New.FinancialInstitutionIdentification;
using InstructedAgent = APGDigitalIntegration.Common.CommonViewModels.Payment_New.InstructedAgent;
using InstructingAgent = APGDigitalIntegration.Common.CommonViewModels.Payment_New.InstructingAgent;

namespace APGDigitalIntegration.IAL.External.Mpcss.Hosts.CBOHostAdapters.Transactional.Outward
{
    public class PaymentReturnOutwardHostAdapter : IPaymentReturnOutwardHostAdapter
    {
        #region Fields

        private readonly IMpcssCommunicator _mpcssCommunicator;
        private readonly IConfParamHelperService _confParamHelperService;
        private readonly IDigitalTransactionRepository _transactionDigitalTrxnAppService;
        private readonly ResponseMessageHandler _messageHandler;
        private readonly ICurrencyApiService _currencyApiService;
        private readonly ITransactionHelper _transactionHelper;
        private readonly IMessageQueue _messageQueue;
        private readonly IMPCSSCommunicationLogService _communicationLogService;
        private readonly ILoggingService _loggingService;
        private readonly IBankApiService _bankApiService;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IMPCSSMessageBuilder _mpcssMessageBuilder;
        private readonly IMerchantMPCSSTransactionRequestsRepository _merchantMPCSSTransactionRequestsRepository;

        #endregion

        #region Constructor 
        public PaymentReturnOutwardHostAdapter(IMpcssCommunicator mpcssCommunicator, 
            IConfParamHelperService confParamHelperService,
            IDigitalTransactionRepository transactionDigitalTrxnAppService,
            ResponseMessageHandler messageHandler,
            ICurrencyApiService currencyApiService,
            ITransactionHelper transactionHelper,
            IMessageQueue messageQueue,
            IMPCSSCommunicationLogService communicationLogService,
            ILoggingService loggingService,
            IBankApiService bankApiService,
            IDateTimeProvider dateTimeProvider ,
            IMPCSSMessageBuilder mpcssMessageBuilder,
            IMerchantMPCSSTransactionRequestsRepository merchantMPCSSTransactionRequestsRepository)
        {
            _mpcssCommunicator = mpcssCommunicator;
            _messageHandler = messageHandler;
            _currencyApiService = currencyApiService;
            _transactionHelper = transactionHelper;
            _messageQueue = messageQueue;
            _communicationLogService = communicationLogService;
            _loggingService = loggingService;
            _bankApiService = bankApiService;
            _dateTimeProvider = dateTimeProvider;
            _mpcssMessageBuilder = mpcssMessageBuilder;
            _merchantMPCSSTransactionRequestsRepository = merchantMPCSSTransactionRequestsRepository;
            _confParamHelperService = confParamHelperService;
            _transactionDigitalTrxnAppService = transactionDigitalTrxnAppService;
        }

        #endregion

        #region Public Methods

        public async Task<ServiceResponse<DigitalTransaction>> Execute(RefundPaymentRequest refundPaymentRequest, OriginalTransactionDetails originalTransactionDetails, TransactionType transactionType)
        {
            DigitalTransaction refundTransaction = default;

            try
            {
                var originalTransaction = await _transactionDigitalTrxnAppService.GetTransaction(new DigitalTransactionFilter(originalTransactionDetails.TransactionId.ToString(), DigitalTransactionIdentifier.DigitalTransactionId));

                refundTransaction = await AddDigitalTransaction(refundPaymentRequest, originalTransaction, originalTransactionDetails);

                var mpcssMessage = MpcssMethods.PopulateMessageType(MPCSSRecordRequest.PaymentReturnRequest);

                var externalRequest = await ConvertToExternalRequest(refundPaymentRequest, refundTransaction.ExternalTransactionId, originalTransaction);
                _communicationLogService.SetRequestDatetime(DateTime.Now);
                _communicationLogService.SetTransactionTypeId(transactionType);
                _communicationLogService.SetExternalRequest(externalRequest);
                await _communicationLogService.SetExternalRequestTime();
                _communicationLogService.SetMsgId(refundTransaction.ExternalTransactionId);
                this._communicationLogService.MPCSSCommunicationLogModel.OriginalExternalMsgId = originalTransaction.ExternalTransactionId;
                _communicationLogService.MPCSSCommunicationLogModel.BankId = refundTransaction.BankId;
                await SendMessage(externalRequest, mpcssMessage.QueueType).ConfigureAwait(false);

                return new ServiceResponse<DigitalTransaction>(success: true, responseCode: ResponseCodes.Success, message: _messageHandler.GetMessage(PaymentSuccessMessage.RequestSentSuccessfully), refundTransaction);
            }
            catch (BusinessException ex)
            {
                var exceptionLogId = await _loggingService.HandleException(ex);
                _communicationLogService.SetExceptionId(exceptionLogId.ToString());
                
                refundTransaction = await FillBusinessFailureDigitalTransaction(refundPaymentRequest, originalTransactionDetails, refundTransaction, ex);

                return new ServiceResponse<DigitalTransaction>(true, ResponseCodes.TechnicalException, _messageHandler.GetMessage(PaymentFailureMessage.RequestInitiationFailed), refundTransaction);
            }
            catch (Exception ex)
            {
                var exceptionLogId = await _loggingService.HandleException(ex);
                _communicationLogService.SetExceptionId(exceptionLogId.ToString());
                
                refundTransaction = await FillTechnicalFailureDigitalTransaction(refundPaymentRequest, originalTransactionDetails, refundTransaction,transactionType);

                return new ServiceResponse<DigitalTransaction>(true, ResponseCodes.TechnicalException, _messageHandler.GetMessage(PaymentFailureMessage.RequestInitiationFailed), refundTransaction);
            }
            finally
            {
                if (refundTransaction!.IdN == default)
                    _transactionDigitalTrxnAppService.Add(refundTransaction);
                    
                await _transactionDigitalTrxnAppService.UnitOfWork.Commit();
            }
        }

        private async Task<DigitalTransaction> FillBusinessFailureDigitalTransaction(RefundPaymentRequest refundPaymentRequest, OriginalTransactionDetails originalTransactionDetails, DigitalTransaction refundTransaction, BusinessException ex)
        {
            var now = await _dateTimeProvider.NowByBankId(refundPaymentRequest.BankId);
            refundTransaction ??= new DigitalTransaction()
            {
                Amount = originalTransactionDetails.Amount,
                CurrencyId = originalTransactionDetails.CurrencyId,
                TransactionTypeId =refundPaymentRequest.TransactionTypeId,
                TerminalNodeId = refundPaymentRequest.TerminalNodeId,
                MerchantRefId = refundPaymentRequest.MerchantRefId,
                Id = refundPaymentRequest.TransactionId,
                BankId = refundPaymentRequest.BankId,
                CreatedDatetime = now,
                MaxResponseDatetime = now,
                IsRefunded = false,
                ExternalTransactionId = null,
                TransactionMethodId = (int)TransactionMethods.WalletECommerce,
                RequestSourceId = refundPaymentRequest.RequestSource,
                AggregatorId = refundPaymentRequest.AggregatorId,
                MerchantAccountTypeId = refundPaymentRequest.SettAccType,
                MerchantId = refundPaymentRequest.MerchantId,
                TerminalId = refundPaymentRequest.TerminalId,
                OriginalTransactionIdN = originalTransactionDetails.TransactionIdN
            };

            refundTransaction.ResponseCode = ex.ResponseCode;
            refundTransaction.Status = ex.Message;
            return refundTransaction;
        }
        private async Task<DigitalTransaction> FillTechnicalFailureDigitalTransaction(RefundPaymentRequest refundPaymentRequest, OriginalTransactionDetails originalTransactionDetails, DigitalTransaction refundTransaction,TransactionType transactionType)
        {
            var now = await _dateTimeProvider.NowByBankId(refundPaymentRequest.BankId);
            refundTransaction ??= new DigitalTransaction
            {
                Amount = originalTransactionDetails.Amount,
                CurrencyId = originalTransactionDetails.CurrencyId,
                TransactionTypeId = (int)TransactionType.P2BRefund,
                TerminalNodeId = refundPaymentRequest.TerminalNodeId,
                MerchantRefId = refundPaymentRequest.MerchantRefId,
                Id = refundPaymentRequest.TransactionId,
                BankId = refundPaymentRequest.BankId,
                CreatedDatetime = now ,
                MaxResponseDatetime = now,
                IsRefunded = false,
                ExternalTransactionId = null,
                TransactionMethodId = (int)TransactionMethods.WalletECommerce,
                RequestSourceId = refundPaymentRequest.RequestSource,
                AggregatorId = refundPaymentRequest.AggregatorId,
                MerchantAccountTypeId = refundPaymentRequest.SettAccType,
                MerchantId = refundPaymentRequest.MerchantId,
                TerminalId = refundPaymentRequest.TerminalId,
                OriginalTransactionIdN = originalTransactionDetails.TransactionIdN
            };

            refundTransaction.ResponseCode = ResponseCodes.Failure;
            refundTransaction.Status = ResponseCodes.TechnicalException;
            return refundTransaction;
        }

        #endregion

        #region Private Methods
        
        private async Task<DigitalTransaction> AddDigitalTransaction(RefundPaymentRequest baseInternalRequest, DigitalTransaction originalTransaction, OriginalTransactionDetails originalTransactionDetails)
        {
            var participantPrefix = await _confParamHelperService.GetValue<string>(ConfigParam.MPCSSPSPRouteCode);
            var now = await _dateTimeProvider.NowByBankId(baseInternalRequest.BankId);
            var request = await _merchantMPCSSTransactionRequestsRepository.Add(new MerchantMPCSSTransactionRequest()
            {
                QROrderId = null,
                UniqueNotificationId = baseInternalRequest.RequestSource == (int)RequestSources.System ? "SYSTEM" : baseInternalRequest.UniqueNotificationId,
                Status = MPCSSStatus.Initiated.ToString(),
                TransactionType = MPCSSRecordRequest.PaymentReturnRequest.ToString(),
                ParticipantPrefix = participantPrefix,
                RequestSourceId = baseInternalRequest.RequestSource,
                CreationDate = now,
                Language = Thread.CurrentThread.CurrentCulture.ToString(),
            });
            await _merchantMPCSSTransactionRequestsRepository.UnitOfWork.Commit();
            var msgIdentificationCode = request.MessageId;

            var mpcssTimeout = await _confParamHelperService.GetValue<short>(ConfigParam.MPCSSOutwardTransactionInternalTimeout);
            var trx = new DigitalTransaction()
            {
                Amount = originalTransaction.Amount,
                Status = TransactionStatus.Processing,
                CurrencyId = originalTransaction.CurrencyId,
                TransactionTypeId = baseInternalRequest.TransactionTypeId,
                TerminalNodeId = baseInternalRequest.TerminalNodeId,
                MerchantRefId = baseInternalRequest.MerchantRefId,
                Id = baseInternalRequest.TransactionId,
                BankId = baseInternalRequest.BankId,
                CreatedDatetime = now,
                MaxResponseDatetime = now.AddSeconds(mpcssTimeout),
                IsRefunded = false,
                ExternalTransactionId = msgIdentificationCode,
                TransactionMethodId = (int)TransactionMethods.WalletECommerce,
                OrderId = Guid.Empty,
                RequestSourceId = baseInternalRequest.RequestSource,
                AggregatorId = baseInternalRequest.AggregatorId,
                MerchantAccountTypeId = baseInternalRequest.SettAccType,
                MerchantId = baseInternalRequest.MerchantId,
                TerminalId = baseInternalRequest.TerminalId,
                OriginalDigitalTransactionIdN = originalTransaction.IdN,
                OriginalTransactionIdN = originalTransactionDetails.TransactionIdN
            };

            _transactionDigitalTrxnAppService.Add(trx);
            await _transactionDigitalTrxnAppService.UnitOfWork.Commit();

            return trx;
        }

        private async Task<Envelope> ConvertToExternalRequest(RefundPaymentRequest refundPaymentRequest, string externalMessageId, DigitalTransaction originalTransaction)
        {
            var bankConfigurations = await _bankApiService.GetBank(refundPaymentRequest.BankId);
            var instructingAgentBICFI = bankConfigurations.Data.BankConfiguration.BankIdentifierCode;

            var now = await _dateTimeProvider.SystemNow();

            var paymentReturnRequest = new MPCSSPaymentReturnRequest()
            {
                GrpHdr = new GroupHeader
                {
                    MsgId = externalMessageId,
                    CreatedDateTime = now.ToISODateTime(),
                    NumberOfTranasctions = "1",
                    InterbankSettlementDate = now.ToISODate(),
                    SettlementInformation = new SettlementInformation()
                    {
                        SettlementMethod = MpcssMessageConstants.Settlement.SttlmMtd,
                        ClearingSystem = new ClearingSystem()
                        {
                            Proprietary = MpcssMessageConstants.Settlement.ClrSysPrtry
                        }
                    },
                    InstructingAgent = new InstructingAgent()
                    {
                        FinancialInstitutionIdentification = new FinancialInstitutionIdentification()
                        {
                            BICFI = instructingAgentBICFI
                        }
                    },
                    InstructedAgent = new InstructedAgent()
                    {
                        FinancialInstitutionIdentification = new FinancialInstitutionIdentification()
                        {
                            BICFI = MpcssMessageConstants.CBO.BICFI
                        }
                    }

                },
                OriginalGroupInformation = new OriginalGroupInformation()
                {
                    OrgnlMsgId = originalTransaction.OriginalExternalTransactionId,
                    OrgnlMsgNmId = MpcssMessageConstants.GetMPCSSMessageTypeByTransactionType((TransactionType)originalTransaction.TransactionTypeId)
                },
                RefundTransactionInformation = new RefundTransactionInformation()
                {
                    RtrId = externalMessageId,
                    OrgnlTxId = originalTransaction.OriginalExternalTransactionId,
                    RtrdIntrBkSttlmAmt = new ActiveAmountAndCurrency
                    {
                        Amount = originalTransaction.Amount,
                        Currency = await _currencyApiService.GetCurrencyShortNameByCurrencyId(originalTransaction.CurrencyId)
                    },
                    RtrRsnInf = new APGMPCSSIntegration.Common.CommonViewModels.Payment.ReturnReasonInformationDto
                    {
                        Rsn = new APGMPCSSIntegration.Common.CommonViewModels.Payment.ReturnReasonDto
                        {
                            Prtry = PaymentRejectionReason.ProcessedSuccessfully.ToString()
                        },
                        AddtlInf = "Return Request"
                    }
                },
                SplmtryData = new SupplementaryData()
                {
                    Envlp = new Envlp()
                    {
                        achSupplementaryData = new AchSupplementaryData()
                        {
                            BatchSource = "2"
                        }
                    }
                }
            };

            var paymentRequestDto = new MPCSSPaymentReturnRequestRoot()
            {
                MPCSSPaymentReturnRequest = paymentReturnRequest
            };

            return _mpcssMessageBuilder.ConvertToExternalRequest(paymentRequestDto, paymentReturnRequest.GrpHdr.CreatedDateTime, externalMessageId, MPCSSMessageTypes.PAYMENT_RETURN_MESSAGE_TYPE , false);
        }
        
        private async Task<ServiceResponse> SendMessage(Envelope message, string queue)
        {
            // Read this key from ConfParams.
            var isSimulated = await _confParamHelperService.GetValue<string>(ConfigParam.SimulateMPCSSTransaction, null).ConfigureAwait(false);

            if (string.IsNullOrEmpty(isSimulated) || isSimulated != "true")
            {
                await _mpcssCommunicator.SendMessage(message, queue, ActiveMQMessageTypes.Text);

                return new ServiceResponse(
                    success: true,
                    responseCode: ResponseCodes.Success,
                    message: _messageHandler.GetMessage(PaymentSuccessMessage.RequestSentSuccessfully));
            }
            else
            {
                // Simulated Environment
                return await ConstructSimulatedResponse(message);
            }
        }

        private async Task<ServiceResponse> ConstructSimulatedResponse(Envelope request)
        {
            var isSimulatedWalletRefundSuccess = await _confParamHelperService.GetValue<bool>(ConfigParam.IsSimulatedWalletRefundSuccess);

            #region Fields
            
            MPCSSPaymentStatusReport response;
            string groupStatus = "ACSP";
            StatusReasonInformation statusInformationResponseDto = null;
            string requestAmount = CommonMethods.GetXMLAttributeValue(XElement.Parse(Convert.ToString(request.Content.Value)), "RtrdIntrBkSttlmAmt");
            string externalMessageId = CommonMethods.GetXMLAttributeValue(XElement.Parse(Convert.ToString(request.Content.Value)), "MsgId");
            
            #endregion

            #region Error Mapping
            if (isSimulatedWalletRefundSuccess == false)
            {
                groupStatus = "RJCT";
                statusInformationResponseDto = new StatusReasonInformation()
                {
                    Reason = new Reason()
                    {
                        OriginalGroupStatusProprietary = "1105"
                    },
                    AdditionalInformation = "Mzagna kda"
                };
            }

            #endregion

            #region XML Construction
            var now = await _dateTimeProvider.SystemNow();

            response = new MPCSSPaymentStatusReport()
            {
                GroupHeader = new GroupHeader()
                {
                    MsgId = externalMessageId,
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
                    }
                },
                OrgnlGrpInfAndSts = new OriginalGroupStatusAndInformation()
                {
                    OriginalMessageId = externalMessageId,
                    OriginalMessageStatus = MPCSSMessageTypes.PAYMENT_RETURN_MESSAGE_TYPE,
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
                            BatchSource = MpcssMessageConstants.SupplementaryData.BatchSource,
                            SessionSequence = MpcssMessageConstants.SupplementaryData.SessionSequence
                        }
                    }
                }
            };

            var paymentReportResponseDto = new MPCSSPaymentStatusReportRoot()
            {
                MPCSSPaymentStatusReport = response
            };

            string datetime = response.GroupHeader.CreatedDateTime;
            var message = _mpcssMessageBuilder.ConvertToExternalRequest(paymentReportResponseDto, datetime, response.OrgnlGrpInfAndSts.OriginalMessageId, response.OrgnlGrpInfAndSts.OriginalMessageStatus, true);
            
            #endregion

            #region Active Mq for Receiver
            Task.Run(() =>
            {
                Thread.Sleep(2000);
                _mpcssCommunicator.SendMessage(message, MPCSSQueues.InwardReplyQueue, ActiveMQMessageTypes.Text);
            }); 
            #endregion
            
            return new ServiceResponse(
                success: true,
                responseCode: ResponseCodes.Success,
                message: _messageHandler.GetMessage(PaymentSuccessMessage.RequestSentSuccessfully));
        }


        #endregion
    }
}
