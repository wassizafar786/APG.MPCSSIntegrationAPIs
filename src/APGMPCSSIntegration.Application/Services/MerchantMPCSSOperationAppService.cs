using APGDigitalIntegration.Application.Interfaces;
using APGDigitalIntegration.IAL.External.Hosts.CBOHosts;
using APGDigitalIntegration.IAL.External.Interfaces.ICBOHosts;
using APGDigitalIntegration.IAL.External.Interfaces;
using System;
using System.Threading.Tasks;
using APGExecutions.IAL.Internal.Interfaces.APGFundamentals;
using APGMPCSSIntegration.Constant;
using APGMPCSSIntegration.Common.CommonViewModels.Response;
using APGMPCSSIntegration.Common.CommonViewModels.Request;
using APGMPCSSIntegration.Common.CommonViewModels.Common;
using APGDigitalIntegration.Common.CommonServices;
using System.Threading;
using APGDigitalIntegration.Common.CommonViewModels.Request;
using Apache.NMS;
using APG.MessageQueue.Contracts.MerchantMPCSSOperations;
using APG.MessageQueue.Interfaces;
using APG.MessageQueue.Mpcss.Interfaces;
using APGDigitalIntegration.Common.CommonMethods.MessageBuilders;
using APGDigitalIntegration.Common.CommonViewModels.Operation.Registration.Response;
using APGDigitalIntegration.Common.CommonViewModels.Response;
using APGDigitalIntegration.Constant;
using APGDigitalIntegration.Domain.Interfaces;
using APGMPCSSIntegration.Common.CommomMethods.MessageBuilders;
using APGMPCSSIntegration.DomainHelper;
using Polly;
using Polly.Retry;
using Polly.CircuitBreaker;
using APGDigitalIntegration.IAL.External.Mpcss.Hosts.SimulateAdapter;
using APGDigitalIntegration.IAL.External.Mpcss.HostsFactories;
using APGDigitalIntegration.IAL.External.Mpcss.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace APGDigitalIntegration.Application.Services
{
    public class MerchantMPCSSOperationAppService : IMerchantMPCSSOperationAppService
    {
        #region Fields
        private readonly IMpcssCommunicator _mpcssCommunicator;
        private readonly IConfParamHelperService _confParamHelperService;
        private readonly ResponseMessageHandler _messageHandler = null;
        private readonly IMPCSSCommunicationLogService _mpcssCommunicationLogService;
        private readonly IMessageQueue _messageQueue;
        private readonly ILoggingService _loggingService;
        private readonly ISimulateHostAdapter _simulateHostAdapter;
        private readonly IMPCSSMessageBuilder _mpcssMessageBuilder;
        private readonly IServiceProvider _serviceProvider;
        private AsyncRetryPolicy _retryPolicy;
        int maxRetryCount = 3;
        double circuitBreakDurationSeconds = 7;

        #endregion
        public MerchantMPCSSOperationAppService(IMpcssCommunicator mpcssCommunicator, IConfParamHelperService confParamHelperService, IMPCSSCommunicationLogService mpcssCommunicationLogService,
            ILoggingService loggingService, IMessageQueue messageQueue, ISimulateHostAdapter simulateHostAdapter, IMPCSSMessageBuilder mpcssMessageBuilder, IServiceProvider serviceProvider)
        {
            _mpcssCommunicator = mpcssCommunicator;
            _confParamHelperService = confParamHelperService;
            _messageHandler = new ResponseMessageHandler();
            _mpcssCommunicationLogService = mpcssCommunicationLogService;
            _loggingService = loggingService;
            _messageQueue = messageQueue;
            _simulateHostAdapter = simulateHostAdapter;
            _mpcssMessageBuilder = mpcssMessageBuilder;
            _serviceProvider = serviceProvider;

            _retryPolicy = Policy.Handle<Exception>().WaitAndRetryAsync(maxRetryCount, retryAttempt => { return TimeSpan.FromSeconds((retryAttempt + 1) + (retryAttempt * circuitBreakDurationSeconds)); });
        }

        public async Task MerchantMPCSSOperationRequest(UpdateMpcssMerchant internalRequest)
        {
            if (internalRequest == null)
                return;

            ServiceResponse resp = null;
            await _mpcssCommunicationLogService.SetInternalRequestTime();
            _mpcssCommunicationLogService.SetInternalRequest(internalRequest);
            _mpcssCommunicationLogService.SetMsgId(internalRequest.MsgId);
            _mpcssCommunicationLogService.SetCorrelationId(internalRequest.CorrelationId);
          //  _mpcssCommunicationLogService.MPCSSCommunicationLogModel.TransactionTypeId = internalRequest.RequestType;
            _mpcssCommunicationLogService.SetRequestDatetime(DateTime.Now);
            _mpcssCommunicationLogService.SetRequstTypeId(internalRequest.RequestType);
            _mpcssCommunicationLogService.MPCSSCommunicationLogModel.BankId = internalRequest.Request.BankId;
            _mpcssCommunicationLogService.MPCSSCommunicationLogModel.MerchantRefId = internalRequest.Request.MerchantRefId;
          
            try
            {
                switch ((MPCSSRecordRequest)internalRequest.RequestType)
                {
                    case MPCSSRecordRequest.RecordOpeningRequest:
                    case MPCSSRecordRequest.RecordMaintenanceRequest:
                    case MPCSSRecordRequest.RecordClosingRequest:
                        var _hostsFactory = new BaseHostFactory<RecordRegistrationHostAdapter>(_serviceProvider);
                        var hostAdapter = _hostsFactory.CreateHost(_mpcssCommunicator, _confParamHelperService, _messageHandler, _mpcssCommunicationLogService,
                            _simulateHostAdapter, _loggingService, _mpcssMessageBuilder);
                        var recordReq = ConstructRecordRequest(internalRequest);
                        resp = await _retryPolicy.ExecuteAsync(async () =>
                            await hostAdapter.Execute(recordReq, (MPCSSRecordRequest)internalRequest.RequestType).ConfigureAwait(false));
                        break;

                    case MPCSSRecordRequest.AccountOpeningRequest:
                    case MPCSSRecordRequest.AccountMaintenanceRequest:
                    case MPCSSRecordRequest.AccountClosingRequest:
                        var _accountHostsFactory = new BaseHostFactory<AccountRegistrationHostAdapter>(_serviceProvider);
                        var accountAdapter = _accountHostsFactory.CreateHost(_mpcssCommunicator, _confParamHelperService, _messageHandler, _mpcssCommunicationLogService, _simulateHostAdapter, _loggingService, _mpcssMessageBuilder);
                        var accountReq = ConstructAccountRequest(internalRequest);
                        resp = await _retryPolicy.ExecuteAsync(async () => await accountAdapter.Execute(accountReq, (MPCSSRecordRequest)internalRequest.RequestType).ConfigureAwait(false));
                        break;
                }
              
            }
            catch (BrokenCircuitException ex)
            {
                var exLog = await _loggingService.HandleException(ex);
                _mpcssCommunicationLogService.SetExceptionId(exLog.ToString());
                await _mpcssCommunicationLogService.Log();

                var internalResponse = new MpcssMerchantUpdateResult
                {
                    Status = (int)MPCSSOperationStatus.Error,
                    OriginalMsgId = internalRequest.MsgId,
                    RequestType = (int)internalRequest.RequestType,
                    ErrorCode = ResponseCodes.Failure,
                    ErrorMessage = ResponseMessages.TechnicalExceptionFailure ,
                    CorrelationId = internalRequest.CorrelationId,
                   
                };

                _mpcssCommunicationLogService.SetInternalResponse(internalResponse);
                await _mpcssCommunicationLogService.SetInternalResponseTime();
                await _messageQueue.PublishMessage(internalResponse, CancellationToken.None);
            }
            finally
            {
                await _mpcssCommunicationLogService.Log();
            }
            
        }
        public async Task MerchantMPCSSOperationResponse(RegistrationResponse response)
        {
            if (response != null)
            {
                var internalResponse = new MpcssMerchantUpdateResult();
                var isAccepted = response.OrgnlMsgSts.Status == MPCSSResponseStatus.ACPT.ToString();
                internalResponse.Status = (int)(isAccepted ? MPCSSOperationStatus.Success : MPCSSOperationStatus.Error);
                internalResponse.MsgId = response.MsgId.Id;
                internalResponse.CorrelationId = response.CorrelationId;
                internalResponse.OriginalMsgId = response.OrgnlMsgId.OriginalMessageId;
                internalResponse.RequestType = (int)MapRequest(response.OrgnlMsgId.OriginalMessageType);
                if (!isAccepted)
                {
                    internalResponse.ErrorCode = response.OrgnlMsgSts.ReasonCode;
                    internalResponse.ErrorMessage = response.OrgnlMsgSts.Narration;
                }

                _mpcssCommunicationLogService.SetInternalResponse(internalResponse);
                await _mpcssCommunicationLogService.SetInternalResponseTime();
                _mpcssCommunicationLogService.MPCSSCommunicationLogModel.ResponseCode = internalResponse.Status.ToString();
                await _messageQueue.PublishMessage(internalResponse, CancellationToken.None);
            }
        }
        public RegistrationRecordInputDto ConstructRecordRequest(UpdateMpcssMerchant model)
        {
            return new RegistrationRecordInputDto
            {
                //<MsgId>
                MessageIdentifier = new MessageIdentifierInputDto { MessageIdentificationCode = model.MsgId },
                CreationDateTime = DateTime.UtcNow,
                ParticipantId = model.Request.ParticipantId, //<PrtcpntId> 
                //</MsgId>

                //<CstmrId>
                CustomerId = model.Request.IdentNumber,
                IdentificationTypeCode = model.Request.IdentCode,
                IdIssuingCountryCode = model.Request.IdentCountryCode,

                CustomerType = MPCSSCustomerType.Merchant.GetEnumDescription(),
                CustomerName = model.Request.MerchantName,
                DobOrRegistrationDate = DateTime.UtcNow,
                //</CstmrId>

                //<Address>
                POBox = model.Request.POBox,
                PostalCode = model.Request.PostalCode,
                StreetName = model.Request.StreetName,
                BuildingNumber = model.Request.BuildingNumber,
                PhoneNumber = model.Request.PhoneNumber,
                MobileNumber = model.Request.MobileNumber,
                CityName = model.Request.CityName,
                TownName = model.Request.TownName,
                GovernorateName = model.Request.GovernorateName,
                CountryCode = model.Request.CountryCode,
                //</Address>

                AdditionalInfo = model.Request.AdditionalInfo,
                BankId = model.Request.BankId.ToString(),
             
            };
        }
        public RegistrationAccountInputDto ConstructAccountRequest(UpdateMpcssMerchant model)
        {
            return new RegistrationAccountInputDto
            {
                //<MsgId>
                MessageIdentifier = new MessageIdentifierInputDto { MessageIdentificationCode = model.MsgId },
                CreationDateTime = DateTime.UtcNow,
                ParticipantId = model.Request.ParticipantId, //<PrtcpntId> 
                //</MsgId>

                //<AcntId>
                MobileOrMerchantId = model.Request.MerchantId, //<MblOrSvc>
                AccountType = MPCSSAccountType.MerchantId.GetEnumDescription(), //<AcntTp>
                RegistrationCode = model.Request.RegistrationCode, //<RgstrtnCd>
                AccountCurrency = model.Request.AccountCurrency, //<Ccy>
                AccountAlias = model.Request.AccountAlias, //<Alias>
                IsDefaultAccount = "True", //<DfltAcnt>                                       
                //</ActId>

                AdditionalInfo = model.Request.AdditionalInfo,
                BankId = model.Request.BankId.ToString(),
            
               
            };
        }

        private MPCSSRecordRequest MapRequest(string externalRequestType)
        {
            switch (externalRequestType)
            {
                case MPCSSMessageTypes.OPEN_CUSTOMER_MESSAGE_TYPE:
                    return MPCSSRecordRequest.RecordOpeningRequest;
                case MPCSSMessageTypes.MAINTAIN_CUSTOMER_MESSAGE_TYPE:
                    return MPCSSRecordRequest.RecordMaintenanceRequest;
                case  MPCSSMessageTypes.CLOSE_CUSTOMER_MESSAGE_TYPE:
                    return MPCSSRecordRequest.RecordClosingRequest;
                case  MPCSSMessageTypes.OPEN_ACCOUNT_MESSAGE_TYPE:
                    return MPCSSRecordRequest.AccountOpeningRequest;
                case  MPCSSMessageTypes.MAINTAIN_ACCOUNT_MESSAGE_TYPE:
                    return MPCSSRecordRequest.AccountMaintenanceRequest;
                case MPCSSMessageTypes.CLOSE_ACCOUNT_MESSAGE_TYPE:
                    return MPCSSRecordRequest.AccountClosingRequest;
                default:
                    return MPCSSRecordRequest.RecordMaintenanceRequest;
            }
        }
    }
}
