using APG.MessageQueue.Mpcss.ActiveMQTransport;
using APG.MessageQueue.Mpcss.Interfaces;
using APGDigitalIntegration.Common.CommonServices;
using APGDigitalIntegration.IAL.External.Interfaces.ICBOHosts;
using APGDigitalIntegration.IAL.External.Mpcss.Hosts.SimulateAdapter;
using APGExecutions.IAL.Internal.Interfaces.APGFundamentals;
using APGMPCSSIntegration.Common.CommomMethods.MessageBuilders;
using APGMPCSSIntegration.Common.CommonViewModels.Registration;
using APGMPCSSIntegration.Common.CommonViewModels.Request;
using APGMPCSSIntegration.Common.CommonViewModels.Response;
using APGMPCSSIntegration.Constant;
using APGMPCSSIntegration.DomainHelper;
using APGDigitalIntegration.Common.CommonMethods.MessageBuilders;
using APGDigitalIntegration.Common.CommonViewModels.Common.OriginalMessageIdentifiers;
using APGDigitalIntegration.Common.CommonViewModels.Operation.Registration.Response;
using APGDigitalIntegration.Common.CommonViewModels.Registeration_New.AccountManagement;
using APGDigitalIntegration.Common.CommonViewModels.Registeration_New.Common;
using APGDigitalIntegration.Common.CommonViewModels.Response;
using APGDigitalIntegration.Constant;
using APGDigitalIntegration.Domain.Interfaces;
using APGDigitalIntegration.DomainHelper.ExtensionMethods;
using APGDigitalIntegration.IAL.External.Mpcss.Communicators;
using APGDigitalIntegration.IAL.External.Mpcss.Interfaces;
using MassTransit;
using Envelope = APGDigitalIntegration.Common.CommonViewModels.Common.Envelope;

namespace APGDigitalIntegration.IAL.External.Hosts.CBOHosts
{
    public class AccountRegistrationHostAdapter : IAccountRegistrationHostAdapter
    {
        #region Fields

        private readonly IMpcssCommunicator _mpcssCommunicator;
        private readonly IConfParamHelperService _confParamHelperService;
        private readonly ResponseMessageHandler _messageHandler = null;
        private readonly IMPCSSCommunicationLogService _mpcssCommunicationLogService;
        private readonly ISimulateHostAdapter _simulateService;
        private readonly ILoggingService _loggingService;
        private readonly IMPCSSMessageBuilder _mpcssMessageBuilder;

        #endregion

        #region Constructor 
        public AccountRegistrationHostAdapter(IMpcssCommunicator mpcssCommunicator, IConfParamHelperService confParamHelperService, ResponseMessageHandler messageHandler, IMPCSSCommunicationLogService mpcssCommunicationLogService, ISimulateHostAdapter simulateService, ILoggingService loggingService, IMPCSSMessageBuilder mpcssMessageBuilder)
        {
            _mpcssCommunicator = mpcssCommunicator;
            _messageHandler = messageHandler;
            _confParamHelperService = confParamHelperService;
            _mpcssCommunicationLogService = mpcssCommunicationLogService;
            _simulateService = simulateService;
            _loggingService = loggingService;
            _mpcssMessageBuilder = mpcssMessageBuilder;
        }

        #endregion

        #region Public Methods

        public async Task<ServiceResponse> Execute(RegistrationAccountInputDto baseInternalRequest, MPCSSRecordRequest mpcssMessageType)
        {
            try
            {
                var MpcssMessage = MpcssMethods.PopulateMessageType(mpcssMessageType);
                var externalRequest = ConvertToExternalRequest(baseInternalRequest, mpcssMessageType, MpcssMessage);
                _mpcssCommunicationLogService.SetExternalRequest(externalRequest);
                await _mpcssCommunicationLogService.SetExternalRequestTime();
                
                var externalResponse = await SendMessage(baseInternalRequest, baseInternalRequest.MessageIdentifier.MessageIdentificationCode, externalRequest, mpcssMessageType, MpcssMessage).ConfigureAwait(false);

                return new ServiceResponse(
                    success: true,
                    responseCode: ResponseCodes.Success,
                    message: _messageHandler.GetMessage(PaymentSuccessMessage.RequestSentSuccessfully));
            }
            catch (Exception ex)
            {
                var exLog = await _loggingService.HandleException(ex);
                _mpcssCommunicationLogService.SetExceptionId(exLog.ToString());
                throw;
            }
        }

        #endregion

        #region Private Methods

        private Envelope ConvertToExternalRequest(RegistrationAccountInputDto mpcssRequest, MPCSSRecordRequest MessageType, MessageRequesitesDto messageRequesites)
        {
            Envelope externalMessage = default;
            string date = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss");

            if (MessageType.Equals(MPCSSRecordRequest.AccountOpeningRequest))
            {
                var AccountOpeningRequest = new CustomerAccountOpeningExternalRequest
                {
                    MsgId = new MessageIdentifier
                    {
                        Id = mpcssRequest.MessageIdentifier.MessageIdentificationCode,
                        CreDtTm = date,
                    },
                    PrtcpntId = mpcssRequest.ParticipantId,
                    CstmrId = new CustomerIdentification
                    {
                        Id = mpcssRequest.CustomerId,
                        IdIssngCtryCd = mpcssRequest.IdIssuingCountryCode,
                        IdTp = mpcssRequest.IdentificationTypeCode
                    },
                    AcntInfo = new AccountInformationDto
                    {
                        AcntId = new AccountIdentifier
                        {
                            AcntTp = mpcssRequest.AccountType,
                            MblOrSvc = mpcssRequest.MobileOrMerchantId,
                            RgstrtnCd = mpcssRequest.RegistrationCode
                        },
                        AcntBnkd = mpcssRequest.IsAccountBanked,
                        Alias = mpcssRequest.AccountAlias,
                        DfltAcnt = mpcssRequest.IsDefaultAccount,
                        Ccy = mpcssRequest.AccountCurrency
                    },
                    AdtnlInf = mpcssRequest.MessageIdentifier.AdditionalInformation
                };
                var request = new CustomerAccountOpeningExternalRequestRoot()
                {
                    CustomerAccountOpeningExternalRequest = AccountOpeningRequest
                };
                externalMessage = _mpcssMessageBuilder.ConvertToExternalRequest(request, date, mpcssRequest.MessageIdentifier.MessageIdentificationCode, messageRequesites.MessageType, false);
            }
            else if (MessageType.Equals(MPCSSRecordRequest.AccountMaintenanceRequest))
            {
                var AccountMaintenanceRequest = new CustomerAccountMaintenanceExternalRequest
                {
                    MsgId = new MessageIdentifier
                    {
                        Id = mpcssRequest.MessageIdentifier.MessageIdentificationCode,
                        CreDtTm = date,
                    },
                    PrtcpntId = mpcssRequest.ParticipantId,
                    AcntInfo = new AccountInformationDto
                    {
                        AcntId = new AccountIdentifier
                        {
                            AcntTp = mpcssRequest.AccountType,
                            MblOrSvc = mpcssRequest.MobileOrMerchantId,
                            RgstrtnCd = mpcssRequest.RegistrationCode
                        },
                        Alias = mpcssRequest.AccountAlias,
                        DfltAcnt = mpcssRequest.IsDefaultAccount
                    },
                    AdtnlInf = mpcssRequest.MessageIdentifier.AdditionalInformation
                };
                var request = new CustomerAccountMaintenanceExternalRequestRoot()
                {
                    CustomerAccountMaintenanceExternalRequest = AccountMaintenanceRequest
                };
                externalMessage = _mpcssMessageBuilder.ConvertToExternalRequest(request, date, mpcssRequest.MessageIdentifier.MessageIdentificationCode, messageRequesites.MessageType, false);
            }
            else if (MessageType.Equals(MPCSSRecordRequest.AccountClosingRequest))
            {
                var AccountClosingRequest = new CustomerAccountClosingExternalRequest
                {
                    MsgId = new MessageIdentifier
                    {
                        Id = mpcssRequest.MessageIdentifier.MessageIdentificationCode,
                        CreDtTm = date,
                    },
                    PrtcpntId = mpcssRequest.ParticipantId,
                    AcntId = new AccountIdentifier
                    {
                        AcntTp = mpcssRequest.AccountType,
                        MblOrSvc = mpcssRequest.MobileOrMerchantId,
                        RgstrtnCd = mpcssRequest.RegistrationCode
                    },
                    AdtnlInf = mpcssRequest.MessageIdentifier.AdditionalInformation
                };
                var request = new CustomerAccountClosingExternalRequestRoot()
                {
                    CustomerAccountClosingExternalRequest = AccountClosingRequest
                };
                externalMessage = _mpcssMessageBuilder.ConvertToExternalRequest(request, date, mpcssRequest.MessageIdentifier.MessageIdentificationCode, messageRequesites.MessageType, false);
            }
            return externalMessage;

        }
        private async Task<ServiceResponse> SendMessage(RegistrationAccountInputDto mpcssRequest, string msgId, Envelope message, MPCSSRecordRequest messageType, MessageRequesitesDto messageRequesitesDto)
        {
            // Read this key from ConfParams.
            var isSimulated = await _confParamHelperService.GetValue<string>(ConfigParam.SimulateMPCSSTransaction, null).ConfigureAwait(false);

            if (string.IsNullOrEmpty(isSimulated) || isSimulated != "true")
            {
                await _mpcssCommunicator.SendMessage(message, messageRequesitesDto.QueueType, ActiveMQMessageTypes.Text);

                return new ServiceResponse(
                    success: true,
                    responseCode: ResponseCodes.Success,
                    message: _messageHandler.GetMessage(PaymentSuccessMessage.RequestSentSuccessfully));
            }

            // Simulated Environment
            return await ConstructSimulatedResponse(msgId, messageRequesitesDto);
        }

        private Task<ServiceResponse> ConstructSimulatedResponse(string msgId, MessageRequesitesDto messageRequesitesDto)
        {
            var response = new RegistrationResponse()
            {
                MsgId = new MessageIdentification()
                {
                    Id = Guid.NewGuid().ToString("N"),
                    CreatedDateTime = DateTime.UtcNow
                },
                CorrelationId = msgId,
                OrgnlMsgId = new OriginalMessageIdentifier()
                {
                    OriginalMessageId = msgId,
                    OriginalMessageType = messageRequesitesDto.MessageType,
                },
                OrgnlMsgSts = new OriginalMessageStatus()
                {
                    Status = MPCSSResponseStatus.ACPT.ToString(),
                    Narration = "",
                    ReasonCode = MPCSSResponseReasonCode.ProcessedSuccessfully.ToString(),
                }
                
            };
            
            var registrationResp = new RegistrationResponseRoot
            {
                RegResp = response
            };
          
            
            var externalMessage = _mpcssMessageBuilder.ConvertToExternalRequest(registrationResp,  DateTime.UtcNow.ToISODateTime(),  response.MsgId.Id,  messageRequesitesDto.MessageType, false);

            Task.Run(async () =>
            {
                Thread.Sleep(1000);
                await _mpcssCommunicator.SendMessage(externalMessage, MPCSSQueues.RegistrationResponseQueue, ActiveMQMessageTypes.Text);
            });
            
            return Task.FromResult(new ServiceResponse(success: true, responseCode: ResponseCodes.Success, message: _messageHandler.GetMessage(PaymentSuccessMessage.RequestSentSuccessfully)));
        }

        
        #endregion
    }
}
