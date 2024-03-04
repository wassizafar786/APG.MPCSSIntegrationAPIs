using System;
using APGDigitalIntegration.Application.Interfaces;
using APGDigitalIntegration.Constant;
using APGExecutions.IAL.Internal.Interfaces.APGFundamentals;
using APGMPCSSIntegration.Common.CommonViewModels.Request;
using APGMPCSSIntegration.Constant;
using System.Threading.Tasks;
using System.Threading;
using APG.MessageQueue.Contracts.Notifications;
using APG.MessageQueue.Interfaces;
using APG.MessageQueue.Mpcss.Interfaces;
using APGDigitalIntegration.Common.CommonServices;
using APGDigitalIntegration.Common.CommonViewModels.Registeration_New.CustomerNameVerification;
using APGMPCSSIntegration.IAL.Internal.Communicator;
using APGDigitalIntegration.IAL.External.Hosts.CBOHosts;
using APGDigitalIntegration.IAL.External.Interfaces;
using APGDigitalIntegration.IAL.External.Interfaces.ICBOHosts;
using APGDigitalIntegration.IAL.External.Mpcss.Hosts.CBOHostAdapters.Operational;
using APGDigitalIntegration.IAL.External.Mpcss.Hosts.SimulateAdapter;
using APGDigitalIntegration.Common.CommonViewModels.Request;
using APGDigitalIntegration.Common.CommonViewModels.Response;
using APGDigitalIntegration.Domain.Interfaces;
using APGDigitalIntegration.IAL.External.Mpcss.Hosts.CBOHostAdapters.Transactional;
using APGDigitalIntegration.IAL.External.Mpcss.HostsFactories;
using APGDigitalIntegration.IAL.External.Mpcss.Interfaces;
using APGDigitalIntegration.IAL.Internal.Communicator;
using APGDigitalIntegration.IAL.Internal.Interfaces.APGFundamental;
using APGDigitalIntegration.IAL.Internal.Services.APGFundamental;
using APGFundamentals.Application.Helper;
using APGDigitalIntegration.Cache.Interfaces;
using System.Text.Json;
using APGDigitalIntegration.DomainHelper;
using Microsoft.Extensions.Localization;
using System.Globalization;
using Microsoft.AspNetCore.Http;

namespace APGMPCSSIntegration.Application.Services
{
    public class DigitalOperationAppService : IDigitalOperationAppService
    {
        #region Fields

        private readonly IMpcssCommunicator _mpcssCommunicator;
        private readonly IConfParamHelperService _confParamHelperService;
        private readonly IBaseDigitalIntegration _baseDigitalIntegration;
        private readonly IApiCaller _apiCaller;
        private readonly ResponseMessageHandler _messageHandler = null;
        private readonly ISimulateHostAdapter _simulateService;
        private readonly IMessageQueue _messageQueue;
        private readonly IMPCSSCommunicationLogService _mpcssCommunicationLogService;
        private readonly IMerchantAppService _merchantAppService;
        private readonly IMerchantOrderApiService _merchantOrderApiService;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IMPCSSMessageBuilder _mpcssMessageBuilder;
        private readonly IServiceProvider _serviceProvider;
        private readonly IMerchantMPCSSTransactionRequestsRepository _merchantMpcssTransactionRequestApiService;
        private readonly ICacheService _cacheService;
        private readonly IStringLocalizer<DigitalOperationAppService> _localizer;

        #endregion

        #region Constructor
        public DigitalOperationAppService(
            IMpcssCommunicator mpcssCommunicator,
            IConfParamHelperService confParamHelperService,
            IBaseDigitalIntegration baseChecks,
            IApiCaller apiCaller,
            ISimulateHostAdapter simulateService,
            IMessageQueue messageQueue,
            IMPCSSCommunicationLogService mpcssCommunicationLogService,
            IMerchantAppService merchantAppService,
            IMerchantOrderApiService merchantOrderApiService,
            IDateTimeProvider dateTimeProvider,
            IMPCSSMessageBuilder mpcssMessageBuilder,
            IServiceProvider serviceProvider,
            IMerchantMPCSSTransactionRequestsRepository merchantMpcssTransactionRequestApiService
,
            ICacheService cacheService,
            IStringLocalizer<DigitalOperationAppService> localizer)
        {
            _mpcssCommunicator = mpcssCommunicator;
            _confParamHelperService = confParamHelperService;
            _baseDigitalIntegration = baseChecks;
            _apiCaller = apiCaller;
            _messageHandler = new ResponseMessageHandler();
            _simulateService = simulateService;
            _messageQueue = messageQueue;
            _mpcssCommunicationLogService = mpcssCommunicationLogService;
            _merchantAppService = merchantAppService;
            _merchantOrderApiService = merchantOrderApiService;
            _dateTimeProvider = dateTimeProvider;
            _mpcssMessageBuilder = mpcssMessageBuilder;
            _serviceProvider = serviceProvider;
            _merchantMpcssTransactionRequestApiService = merchantMpcssTransactionRequestApiService;
            _cacheService = cacheService;
            _localizer = localizer;
        }

        #endregion

        #region Services

        public async Task<ServiceResponse> SendPaymentStatusReportRequest(PaymentStatusReportInputDto paymentStatusReportInput, MPCSSRecordRequest mpcssMessageType)
        {
            _mpcssCommunicationLogService.MarkValidationsAsPassed();
            var response = new ServiceResponse();

            #region Payment Gateway

            var paymentGateway = await _baseDigitalIntegration.GetPaymentGateway();

            #endregion
            #region External Host
            switch (paymentGateway)
            {
                case PaymentGateways.MPCSS:
                    IBaseHostFactory<PaymentStatusReportHostAdapter> _hostsFactory = new BaseHostFactory<PaymentStatusReportHostAdapter>(_serviceProvider);
                    IPaymentStatusReportHostAdapter hostAdapter = _hostsFactory.CreateHost(_mpcssCommunicator, _confParamHelperService, _messageHandler, _simulateService);
                    response = await hostAdapter.Execute(paymentStatusReportInput, mpcssMessageType).ConfigureAwait(false);
                    break;

                default:
                    break;
            }


            #endregion

            return response;
        }
        public async Task<ServiceResponse> SendCustomerNameVerificationRequest(CustomerNameVerificationRequest customerNameVerificationInput)
        {
            _mpcssCommunicationLogService.MarkValidationsAsPassed();
            var response = new ServiceResponse();

            #region Payment Gateway

            var paymentGateway = await _baseDigitalIntegration.GetPaymentGateway();

            #endregion

            #region External Host
            switch (paymentGateway)
            {
                case PaymentGateways.MPCSS:

                    IBaseHostFactory<RecordVerificationHostAdapter> _hostsFactory = new BaseHostFactory<RecordVerificationHostAdapter>(_serviceProvider);
                    IRecordVerificationHostAdapter hostAdapter = _hostsFactory.CreateHost();
                    response = await hostAdapter.Execute(customerNameVerificationInput).ConfigureAwait(false);
                    break;

                default:
                    break;
            }

            #endregion

            return response;
        }
        public async Task<ServiceResponse> SendDefaultAccountVerificationRequest(DefaultAccountInputDto defaultAccountInputDto, MPCSSRecordRequest mpcssMessageType)
        {
            _mpcssCommunicationLogService.MarkValidationsAsPassed();
            var response = new ServiceResponse();

            #region Payment Gateway

            var paymentGateway = await _baseDigitalIntegration.GetPaymentGateway();

            #endregion

            #region External Host

            switch (paymentGateway)
            {
                case PaymentGateways.MPCSS:
                    IBaseHostFactory<AccountVerificationHostAdapter> _hostsFactory = new BaseHostFactory<AccountVerificationHostAdapter>(_serviceProvider);
                    IAccountVerificationHostAdapter hostAdapter = _hostsFactory.CreateHost(_mpcssCommunicator, _confParamHelperService, _messageHandler, _simulateService, _mpcssMessageBuilder);
                    response = await hostAdapter.Execute(defaultAccountInputDto, mpcssMessageType).ConfigureAwait(false);
                    break;

                default:
                    break;
            }

            #endregion

            return response;

        }

        public async Task ReceiveCustomerNameResponse(CustomerNameVerificationExternalResponse nameVerificationExternalResponse)
        {
            var isSuccess = nameVerificationExternalResponse.IsVerificationSuccess();
            var responseCode = isSuccess
                ? ResponseCodes.Success
                : _messageHandler.MapToAPGResponseCode(Enum.Parse<PaymentRejectionReason>(nameVerificationExternalResponse.OrgnlMsgSts.ReasonCode ?? PaymentRejectionReason.TechnicalError.ToString()));

            


            var messageId = nameVerificationExternalResponse.OrgnlMsgId.OriginalMessageId;
            var response = await _merchantMpcssTransactionRequestApiService.GetByMessageId(messageId);

            var culture = new CultureInfo(response.Language??"en-US");
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;

            var reason = isSuccess
                ? string.Empty
                : _localizer[ _messageHandler.GetMessage(Enum.Parse<PaymentRejectionReason>(nameVerificationExternalResponse.OrgnlMsgSts.ReasonCode ?? PaymentRejectionReason.TechnicalError.ToString()))];

            BaseResponse<CustomerNameVerificationInternalResponse> notificationRequest;
            if (isSuccess)
                notificationRequest = BaseNotificationResponse<CustomerNameVerificationInternalResponse>.GetSuccessResponse(new CustomerNameVerificationInternalResponse
                {
                    CustomerName = nameVerificationExternalResponse.CustomerName,
                    CustomerType = nameVerificationExternalResponse.CustomerType
                }, response.UniqueNotificationId);
            else
                notificationRequest = BaseNotificationResponse<CustomerNameVerificationInternalResponse>.GetFailureResponse(reason, ResponseMessages.ResponseFailure, responseCode, response.UniqueNotificationId);

            _mpcssCommunicationLogService.SetMsgId(messageId);
            _mpcssCommunicationLogService.SetExternalResponseObj(nameVerificationExternalResponse);
            await _mpcssCommunicationLogService.SetExternalResponseTime();

            var eventFrom = NotificationHubs.GetNotificationEventHub(response.RequestSourceId);
            if (eventFrom != null)
            {
                await _messageQueue.PublishMessage(new SendPushNotification(
                    userId: "",
                    sessionId: response.UniqueNotificationId,
                    eventFrom: eventFrom,
                    NotificationHub.DIVerifyCustomerGroup,
                    notificationRequest
                ), CancellationToken.None);
            }
            else if (response.RequestSourceId is (int)RequestSources.MerchantApp or (int)RequestSources.MerchantAppSDK)
            {
                await _cacheService.Publish(messageId, JsonSerializer.Serialize(notificationRequest));
            }
            _mpcssCommunicationLogService.SetInternalResponse(notificationRequest);
            await _mpcssCommunicationLogService.SetInternalResponseTime();
        }


        #endregion

    }
}
