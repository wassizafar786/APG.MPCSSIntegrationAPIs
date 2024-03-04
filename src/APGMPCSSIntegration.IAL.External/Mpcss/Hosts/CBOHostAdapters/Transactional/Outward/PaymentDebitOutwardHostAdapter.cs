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
using APGDigitalIntegration.Common.CommonViewModels.Request;
using APGDigitalIntegration.Common.CommonViewModels.Response;
using APGDigitalIntegration.Common.Observers;
using APGDigitalIntegration.Constant;
using APGDigitalIntegration.Domain.Interfaces;
using APGDigitalIntegration.Domain.Models;
using APGDigitalIntegration.DomainHelper.ExtensionMethods;
using APGDigitalIntegration.DomainHelper.Interfaces;
using APGDigitalIntegration.DomainHelper.Services;
using APGDigitalIntegration.IAL.External.Mpcss.Interfaces;
using APGDigitalIntegration.IAL.External.Mpcss.Interfaces.ICBOHostAdapters.Transactional.Outward;
using APGDigitalIntegration.IAL.Internal.Interfaces.APGFundamental;
using APGDigitalIntegration.IAL.Internal.Interfaces.APGTransaction;
using APGExecutions.IAL.Internal.Interfaces.APGFundamentals;
using APGFundamentals.Application.Helper;
using APGMPCSSIntegration.Common.CommomMethods.MessageBuilders;
using APGMPCSSIntegration.Constant;
using APGMPCSSIntegration.DomainHelper.Exceptions;
using Envelope = APGDigitalIntegration.Common.CommonViewModels.Common.Envelope;
using FinancialInstitutionIdentification = APGDigitalIntegration.Common.CommonViewModels.Payment_New.FinancialInstitutionIdentification;
using PaymentTypeInformation = APGDigitalIntegration.Common.CommonViewModels.Payment_New.PaymentTypeInformation;
using TransactionStatus = APGMPCSSIntegration.Constant.TransactionStatus;


namespace APGDigitalIntegration.IAL.External.Mpcss.Hosts.CBOHostAdapters.Transactional.Outward
{
    public class PaymentDebitOutwardHostAdapter : IPaymentDebitOutwardHostAdapter
    {
        #region Fields


        private readonly IMpcssCommunicator _mpcssCommunicator;
        private readonly IConfParamHelperService _confParamHelperService;
        private readonly ISimulatedReceiver _simulatedReceiver;
        private readonly ICommonTransactionalAppService _commonTransactionalAppService;
        private readonly ResponseMessageHandler _messageHandler = null;
        private readonly ITransactionHelper _transactionHelper;
        private readonly IMessageQueue _messageQueue;
        private readonly ICurrencyApiService _currencyApiService;
        private readonly IMerchantAppService _merchantAppService;
        private readonly IMerchantMPCSSTransactionRequestsRepository _merchantMPCSSTransactionRequestsRepository;
        private readonly IMPCSSCommunicationLogService _communicationLogAppService;
        private readonly ILoggingService _loggingService;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IMPCSSMessageBuilder _mpcssMessageBuilder;
        private readonly IDigitalTransactionRepository _digitalTransactionRepository;

        #endregion

        #region Constructor 
        public PaymentDebitOutwardHostAdapter(IMpcssCommunicator mpcssCommunicator,
            IConfParamHelperService confParamHelperService,
            ISimulatedReceiver simulatedReceiver,
            ICommonTransactionalAppService commonTransactionalAppService,
            ResponseMessageHandler messageHandler,
            ITransactionHelper transactionHelper,
            IMessageQueue messageQueue,
            ICurrencyApiService currencyApiService,
            IMerchantAppService merchantAppService,
            IMerchantMPCSSTransactionRequestsRepository merchantMPCSSTransactionRequestsRepository,
            IMPCSSCommunicationLogService communicationLogAppService,
            ILoggingService loggingService, IDateTimeProvider dateTimeProvider, IMPCSSMessageBuilder mpcssMessageBuilder, IDigitalTransactionRepository digitalTransactionRepository)
        {
            _mpcssCommunicator = mpcssCommunicator;
            _messageHandler = messageHandler;
            _transactionHelper = transactionHelper;
            _messageQueue = messageQueue;
            _currencyApiService = currencyApiService;
            _merchantAppService = merchantAppService;
            _merchantMPCSSTransactionRequestsRepository = merchantMPCSSTransactionRequestsRepository;
            _communicationLogAppService = communicationLogAppService;
            _loggingService = loggingService;
            _dateTimeProvider = dateTimeProvider;
            _mpcssMessageBuilder = mpcssMessageBuilder;
            _digitalTransactionRepository = digitalTransactionRepository;
            _confParamHelperService = confParamHelperService;
            _simulatedReceiver = simulatedReceiver;
            _commonTransactionalAppService = commonTransactionalAppService;
        }

        #endregion

        #region Public Methods

        public async Task<ServiceResponse<DigitalTransaction>> Execute(DebitPaymentInternalRequest baseInternalRequest, Guid? orderId, APGMPCSSIntegration.Constant.TransactionType transactionType)
        {
            DigitalTransaction digitalTransaction = default;
            try
            {
                digitalTransaction = await AddDigitalTransaction(baseInternalRequest, orderId, transactionType);

                var mpcssMessage = MpcssMethods.PopulateMessageType(MPCSSRecordRequest.PaymentOutwardDebitRequest);

                var externalRequest = await ConvertToExternalRequest(baseInternalRequest, digitalTransaction.ExternalTransactionId);
                this._communicationLogAppService.SetRequestDatetime(DateTime.Now);
                this._communicationLogAppService.SetTransactionTypeId(transactionType);
                this._communicationLogAppService.SetExternalRequest(externalRequest);
                await this._communicationLogAppService.SetExternalRequestTime();
                this._communicationLogAppService.SetMsgId(digitalTransaction.ExternalTransactionId);
                this._communicationLogAppService.MPCSSCommunicationLogModel.BankId = baseInternalRequest.BankId;

                await SendMessage(externalRequest, mpcssMessage.QueueType).ConfigureAwait(false);

                return new ServiceResponse<DigitalTransaction>(true, ResponseCodes.Success, _messageHandler.GetMessage(PaymentSuccessMessage.RequestSentSuccessfully), digitalTransaction);
            }
            catch (BusinessException ex)
            {
                var exceptionLogId = await _loggingService.HandleException(ex);
                _communicationLogAppService.SetExceptionId(exceptionLogId.ToString());

                digitalTransaction = await FillBusinessFailureDigitalTransaction(baseInternalRequest, orderId, digitalTransaction, ex, transactionType);

                return new ServiceResponse<DigitalTransaction>(false, ex.ResponseCode ?? ResponseCodes.TechnicalException, ex.ErrorList.FirstOrDefault(), digitalTransaction);
            }
            catch (Exception ex)
            {
                var exceptionLogId = await _loggingService.HandleException(ex);
                _communicationLogAppService.SetExceptionId(exceptionLogId.ToString());

                digitalTransaction = await FillTechnicalFailureDigitalTransaction(baseInternalRequest, orderId, digitalTransaction, transactionType);

                return new ServiceResponse<DigitalTransaction>(false, ResponseCodes.TechnicalException, _messageHandler.GetMessage(PaymentFailureMessage.RequestInitiationFailed), digitalTransaction);
            }
            finally
            {
                if (digitalTransaction!.IdN == default)
                    _digitalTransactionRepository.Add(digitalTransaction);
                    
                await _digitalTransactionRepository.UnitOfWork.Commit();
            }
        }

        private async Task<DigitalTransaction> AddDigitalTransaction(DebitPaymentInternalRequest baseInternalInternalRequest, Guid? orderId, APGMPCSSIntegration.Constant.TransactionType transactionType)
        {
            var participantPrefix = await _confParamHelperService.GetValue<string>(ConfigParam.MPCSSPSPRouteCode);

            var now = await _dateTimeProvider.NowByBankId(baseInternalInternalRequest.BankId);

            var request = await _merchantMPCSSTransactionRequestsRepository.Add(new MerchantMPCSSTransactionRequest()
            {
                QROrderId = null,
                UniqueNotificationId = baseInternalInternalRequest.UniqueNotificationId,
                Status = MPCSSStatus.Initiated.ToString(),
                TransactionType = MPCSSRecordRequest.PaymentOutwardDebitRequest.ToString(),
                ParticipantPrefix = participantPrefix,
                RequestSourceId = (int)baseInternalInternalRequest.RequestSource,
                PaymentViewType=baseInternalInternalRequest.PaymentViewType,
                CreationDate=now,
                Language = Thread.CurrentThread.CurrentCulture.ToString(),
            });

            await _merchantMPCSSTransactionRequestsRepository.UnitOfWork.Commit();
            var msgIdentificationCode = request.MessageId;

            var mpcssTimeout = await _confParamHelperService.GetValue<short>(ConfigParam.MPCSSOutwardTransactionInternalTimeout);
            var trx = new DigitalTransaction
            {
                Amount = baseInternalInternalRequest.Amount,
                Status = TransactionStatus.Processing,
                CurrencyId = baseInternalInternalRequest.CurrencyId,
                TransactionTypeId = (int)TransactionType.P2BPull,
                TerminalNodeId = baseInternalInternalRequest.TerminalNodeId,
                MerchantRefId = baseInternalInternalRequest.MerchantRefId,
                Id = baseInternalInternalRequest.Id,
                BankId = baseInternalInternalRequest.BankId,
                CreatedDatetime = now,
                MaxResponseDatetime = now.AddSeconds(mpcssTimeout),
                IsRefunded = false,
                ExternalTransactionId = msgIdentificationCode,
                TransactionMethodId = baseInternalInternalRequest.TransactionMethodId,
                OrderId = orderId ?? Guid.Empty,
                RequestSourceId = (int)baseInternalInternalRequest.RequestSource,
                AggregatorId = baseInternalInternalRequest.AggregatorId,
                MerchantAccountTypeId = baseInternalInternalRequest.SettAccType,
                MerchantId = baseInternalInternalRequest.MerchantId,
                TerminalId = baseInternalInternalRequest.TerminalId,
                From = baseInternalInternalRequest.MobileNumber,
                SenderName = baseInternalInternalRequest.AliasName,
                SenderMobileNo = baseInternalInternalRequest.MobileNumber,
                MerchantBranchId = baseInternalInternalRequest.MerchantBranchId.GetValueOrDefault(),
            };

            _digitalTransactionRepository.Add(trx);
            await _digitalTransactionRepository.UnitOfWork.Commit();

            return trx;
        }

        private async Task<DigitalTransaction> FillTechnicalFailureDigitalTransaction(DebitPaymentInternalRequest baseInternalInternalRequest, Guid? orderId, DigitalTransaction digitalTransactionModel, TransactionType transactionType)
        {
            var now = await _dateTimeProvider.NowByBankId(baseInternalInternalRequest.BankId);

            digitalTransactionModel ??= new DigitalTransaction()
            {
                Amount = baseInternalInternalRequest.Amount,
                CurrencyId = baseInternalInternalRequest.CurrencyId,
                TransactionTypeId = (int)TransactionType.P2BPull,
                TerminalNodeId = baseInternalInternalRequest.TerminalNodeId,
                MerchantRefId = baseInternalInternalRequest.MerchantRefId,
                Id = baseInternalInternalRequest.Id,
                BankId = baseInternalInternalRequest.BankId,
                CreatedDatetime = now,
                MaxResponseDatetime = now,
                IsRefunded = false,
                ExternalTransactionId = null,
                TransactionMethodId = baseInternalInternalRequest.TransactionMethodId,
                OrderId = orderId ?? Guid.Empty,
                RequestSourceId = (int)baseInternalInternalRequest.RequestSource,
                AggregatorId = baseInternalInternalRequest.AggregatorId,
                MerchantAccountTypeId = baseInternalInternalRequest.SettAccType,
                MerchantId = baseInternalInternalRequest.MerchantId,
                TerminalId = baseInternalInternalRequest.TerminalId,
                From = baseInternalInternalRequest.MobileNumber,
                SenderName = baseInternalInternalRequest.AliasName,
                SenderMobileNo = baseInternalInternalRequest.MobileNumber,
            };

            digitalTransactionModel.ResponseCode = ResponseCodes.TechnicalException;
            digitalTransactionModel.Status = ResponseMessages.ResponseFailure;
            return digitalTransactionModel;
        }

        private async Task<DigitalTransaction> FillBusinessFailureDigitalTransaction(DebitPaymentInternalRequest baseInternalInternalRequest, Guid? orderId, DigitalTransaction digitalTransactionModel, BusinessException ex, TransactionType transactionType)
        {
            var now = await _dateTimeProvider.NowByBankId(baseInternalInternalRequest.BankId);
            digitalTransactionModel ??= new DigitalTransaction()
            {
                Amount = baseInternalInternalRequest.Amount,
                CurrencyId = baseInternalInternalRequest.CurrencyId,
                TransactionTypeId = (int)TransactionType.P2BPull,
                TerminalNodeId = baseInternalInternalRequest.TerminalNodeId,
                MerchantRefId = baseInternalInternalRequest.MerchantRefId,
                Id = baseInternalInternalRequest.Id,
                BankId = baseInternalInternalRequest.BankId,
                CreatedDatetime = now,
                MaxResponseDatetime = now,
                IsRefunded = false,
                ExternalTransactionId = null,
                TransactionMethodId = baseInternalInternalRequest.TransactionMethodId,
                OrderId = orderId ?? Guid.Empty,
                RequestSourceId = (int)baseInternalInternalRequest.RequestSource,
                AggregatorId = baseInternalInternalRequest.AggregatorId,
                MerchantAccountTypeId = baseInternalInternalRequest.SettAccType,
                MerchantId = baseInternalInternalRequest.MerchantId,
                TerminalId = baseInternalInternalRequest.TerminalId,
                From = baseInternalInternalRequest.MobileNumber,
                SenderName = baseInternalInternalRequest.AliasName,
                SenderMobileNo = baseInternalInternalRequest.MobileNumber
            };

            digitalTransactionModel.ResponseCode = ex.ResponseCode;
            digitalTransactionModel.Status = ResponseMessages.ResponseFailure;
            return digitalTransactionModel;
        }

        #endregion

        #region Private Methods

        private async Task<Envelope> ConvertToExternalRequest(DebitPaymentInternalRequest mpcssInternalRequest, string msgId)
        {
            var merchantMpcssAccountData = await _merchantAppService.GetMPCSSAccountPaymentDataModel(mpcssInternalRequest.MerchantRefId);
            var currencyShortName = await _currencyApiService.GetCurrencyShortNameByCurrencyId(mpcssInternalRequest.CurrencyId);
            var creditorAccountIdentificationId = await GetCreditorIdentificationId(merchantMpcssAccountData.MPCSSMerchantId);
            var debtorAccountIdentificationId = GetDebtorIdentificationId(mpcssInternalRequest);

            var customerDirectDebitRequest = new FIToFICstmrDrctDbt();
            var grpHdr = new GroupHeader
            {
                MsgId = msgId,
                CreatedDateTime = DateTime.UtcNow.ToISODateTime(),
                NumberOfTranasctions = "1",
                TotalInterbankSettlementAmount = new TotalInterBankSettlementAmount
                {
                    Value = mpcssInternalRequest.Amount.ToString(),
                    Currency = currencyShortName
                },
                InterbankSettlementDate = DateTime.Today.ToISODate(),
                SettlementInformation = new SettlementInformation
                {
                    SettlementMethod = MpcssMessageConstants.Settlement.SttlmMtd,
                    ClearingSystem = new ClearingSystem
                    {
                        Proprietary = MpcssMessageConstants.Settlement.ClrSysPrtry
                    }
                },
                PaymentTypeInformation = new PaymentTypeInformation
                {
                    LocalInstrument = new LocalInstrument
                    {
                        Code = MpcssMessageConstants.PaymentTypeInformation.LclInstrm
                    },
                    CategoryPurpose = new CategoryPurpose
                    {
                        Proprietary = MpcssMessageConstants.CBO.CategoryPurpose // Move To Conf Parameter
                    }
                },
                InstructingAgent = new InstructingAgent
                {
                    FinancialInstitutionIdentification = new FinancialInstitutionIdentification
                    {
                        BICFI = merchantMpcssAccountData.BankIdentificationCode
                    }
                },
                InstructedAgent = new InstructedAgent
                {
                    FinancialInstitutionIdentification = new FinancialInstitutionIdentification
                    {
                        BICFI = MpcssMessageConstants.CBO.BICFI
                    }
                }
            };

            //Debit Transfer Data
            var drctDbtTxInf = new DebitTransferTransactionInformation
            {
                PaymentIdentification = new PaymentIdentification
                {
                    EndToEndId = Guid.NewGuid().ToString("N"),
                    TransactionId = msgId
                },
                InterbankSettlementAmount = new InterBankSettlementAmount
                {
                    Value = mpcssInternalRequest.Amount.ToString(),
                    Currency = currencyShortName
                },
                ChargeBearer = MpcssMessageConstants.Settlement.ChrgBr,

                Creditor = new Creditor(),
                CreditorAccount = new CreditorAccount
                {
                    Id = new Id
                    {
                        Other = new Other
                        {
                            Id = creditorAccountIdentificationId
                        }
                    }
                },
                CreditorAgent = new CreditorAgent
                {
                    FinancialInstitutionIdentification = new FinancialInstitutionIdentification
                    {
                        BICFI = merchantMpcssAccountData.BankIdentificationCode
                    }
                },

                Debtor = new Debtor
                {
                    // Nm = request.ReceiverName,
                    // Id = new Identification()
                    // {
                    //     PrvtId = new PrivateIdentification()
                    //     {
                    //         Othr = new OtherIdentification()
                    //         {
                    //             Id = request.ReceiverId,
                    //             SchmeNm = new SchemeName()
                    //             {
                    //                 Prtry = request.SchemaProprietary
                    //             },
                    //             Issr = request.Issuer
                    //         }
                    //     }
                    // }
                },

                DebtorAccount = new DebtorAccount
                {
                    Id = new Id
                    {
                        Other = new Other
                        {
                            Id = debtorAccountIdentificationId
                        }
                    }
                },

                DebtorAgent = new DebtorAgent
                {
                    FinancialInstitutionIdentification = new FinancialInstitutionIdentification
                    {
                        BICFI = MpcssMessageConstants.CBO.BICFI
                    }
                }
            };

            var splmtryData = new SupplementaryData
            {
                PlcAndNm = MpcssMessageConstants.SupplementaryData.PlcAndName,
                Envlp = new Envlp
                {
                    achSupplementaryData = new AchSupplementaryData
                    {
                        SessionSequence = MpcssMessageConstants.SupplementaryData.SessionSequence,
                        BatchSource = MpcssMessageConstants.SupplementaryData.BatchSource
                    }
                }
            };

            customerDirectDebitRequest.GroupHeader = grpHdr;
            customerDirectDebitRequest.CreditTransferTransactionInformation = drctDbtTxInf;
            customerDirectDebitRequest.SupplementaryData = splmtryData;

            var paymentRequestDto = new MPCSSPaymentDebitRequest
            {
                FIToFICstmrDrctDbt = customerDirectDebitRequest
            };

            return _mpcssMessageBuilder.ConvertToExternalRequest(paymentRequestDto, grpHdr.CreatedDateTime, msgId, MPCSSMessageTypes.DEBIT_MESSAGE_TYPE, true);
        }

        private async Task<string> GetCreditorIdentificationId(string merchantId)
        {
            var pspRoutingKey = await _confParamHelperService.GetValue<string>(ConfigParam.MPCSSPSPRouteCode);
            var mobileAccountSelector = await _confParamHelperService.GetValue<string>(ConfigParam.MobileAccountSelector);

            return $"{pspRoutingKey}{mobileAccountSelector}{MPCSSAccountIdentificationTypes.MerchantIdIdentification.Identifier}{merchantId}";
        }

        private string GetDebtorIdentificationId(DebitPaymentInternalRequest debitPaymentInternalRequest)
        {
            var identificationType = MPCSSAccountIdentificationTypes.GetByTransactionMethod((TransactionMethods)debitPaymentInternalRequest.TransactionMethodId);

            var identificationValue = identificationType == MPCSSAccountIdentificationTypes.AliasNameIdentification
                ? debitPaymentInternalRequest.AliasName
                : debitPaymentInternalRequest.MobileNumber;

            return $"XXXXXX{identificationType.Identifier}{identificationValue.Replace(" ", "")}";
        }


        private async Task<ServiceResponse> SendMessage(Envelope message, string queue)
        {
            // Read this key from ConfParams.
            var isSimulated = await _confParamHelperService.GetValue<string>(ConfigParam.SimulateMPCSSTransaction).ConfigureAwait(false);

            if (string.IsNullOrEmpty(isSimulated) || isSimulated != "true")
            {
                await _mpcssCommunicator.SendMessage(message, queue, ActiveMQMessageTypes.Text);
                return new ServiceResponse(true, ResponseCodes.Success, _messageHandler.GetMessage(PaymentSuccessMessage.RequestSentSuccessfully));
            }

            // transaction cases on Simulation
            return await ConstructSimulatedResponse(message);
        }

        private async Task<ServiceResponse> ConstructSimulatedResponse(Envelope request)
        {
            var simulatedTransactionCase = await _confParamHelperService.GetValue<SimulatedDigitalTransactionResponse>(ConfigParam.SimulatedDigitalTransactionResponse);
            if (simulatedTransactionCase is SimulatedDigitalTransactionResponse.NoResponseReceived)
                return new ServiceResponse(true, ResponseCodes.Success, _messageHandler.GetMessage(PaymentSuccessMessage.RequestSentSuccessfully));

            var mpcssTimeOutInSeconds = await _confParamHelperService.GetValue<int>(ConfigParam.MPCSSOutwardTransactionInternalTimeout);
            var mpcssTimeOutSeconds = await _confParamHelperService.GetValue<int>(ConfigParam.MPCSSTransactionTimeoutInSeconds);

            #region Fields

            StatusReasonInformation statusInformationResponseDto = null;
            var requestAmount = CommonMethods.GetXMLAttributeValue(XElement.Parse(request.Content.Value), "TotalInterbankSettelmentAmount");
            var msgId = CommonMethods.GetXMLAttributeValue(XElement.Parse(request.Content.Value), "MsgId");

            #endregion

            var groupStatus = simulatedTransactionCase is SimulatedDigitalTransactionResponse.LateSuccessResponseReceived or SimulatedDigitalTransactionResponse.None or SimulatedDigitalTransactionResponse.SuccessResponseReceived
                ? MpcssMessageConstants.ResponseStatus.Accepted
                : MpcssMessageConstants.ResponseStatus.Rejected;

            if (simulatedTransactionCase is SimulatedDigitalTransactionResponse.TimeoutResponseReceived)
                statusInformationResponseDto = new StatusReasonInformation
                {
                    Reason = new Reason()
                    {
                        OriginalGroupStatusProprietary = "1105"
                    },
                    AdditionalInformation = "Reply timeout reached"
                };

            else if (groupStatus == MpcssMessageConstants.ResponseStatus.Rejected)
                statusInformationResponseDto = new StatusReasonInformation
                {
                    Reason = new Reason()
                    {
                        OriginalGroupStatusProprietary = "1001"
                    },
                    AdditionalInformation = "not enough funds"
                };

            #region XML Construction
            var response = new MPCSSPaymentStatusReport()
            {
                GroupHeader = new GroupHeader()
                {
                    MsgId = "TST307092020019",
                    CreatedDateTime = DateTime.UtcNow.ToISODateTime(),
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
                    OriginalMessageId = msgId,
                    OriginalMessageStatus = MPCSSMessageTypes.DEBIT_MESSAGE_TYPE,
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
                            SessionSequence = "314",
                            //GroupMerchantId = grpMerchId,
                            //TerminalId = terminalId
                        }
                    }
                }
            };

            var paymentReportResponseDto = new MPCSSPaymentStatusReportRoot()
            {
                MPCSSPaymentStatusReport = response
            };



            var datetime = response.GroupHeader.CreatedDateTime;

            var message = _mpcssMessageBuilder.ConvertToExternalRequest(paymentReportResponseDto, datetime, response.OrgnlGrpInfAndSts.OriginalMessageId, response.OrgnlGrpInfAndSts.OriginalMessageStatus, true);
            var messageText = XmlSerializationHelper.Serialize(message);

            #endregion

            #region Active mq for Receiver
            Task.Run(async () =>
            {
                if (simulatedTransactionCase is SimulatedDigitalTransactionResponse.LateFailureResponseReceived or SimulatedDigitalTransactionResponse.LateSuccessResponseReceived)
                    await Task.Delay(mpcssTimeOutSeconds * 1000 - 3000); // i.e if 'MPCSSTransactionTimeoutInSeconds' from ConfParam value is 25 then its is 25 + 5
                else if (simulatedTransactionCase is SimulatedDigitalTransactionResponse.TimeoutResponseReceived)
                    await Task.Delay(mpcssTimeOutInSeconds * 1000 - 3000);
                else
                    await Task.Delay(2000);

                await _mpcssCommunicator.SendMessage(message, MPCSSQueues.InwardReplyQueue, ActiveMQMessageTypes.Text);
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
