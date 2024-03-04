using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text.Json;
using System.Xml.Linq;
using APG.MessageQueue.Interfaces;
using APG.MessageQueue.Mpcss.ActiveMQTransport;
using APG.MessageQueue.Mpcss.Interfaces;
using APGDigitalIntegration.Cache.Interfaces;
using APGDigitalIntegration.Common.CommonMethods;
using APGDigitalIntegration.Common.CommonMethods.MessageBuilders;
using APGDigitalIntegration.Common.CommonServices;
using APGDigitalIntegration.Common.CommonViewModels.Common.OriginalMessageIdentifiers;
using APGDigitalIntegration.Common.CommonViewModels.Payment_New;
using APGDigitalIntegration.Common.CommonViewModels.Registeration_New;
using APGDigitalIntegration.Common.CommonViewModels.Registeration_New.CustomerNameVerification;
using APGDigitalIntegration.Common.CommonViewModels.Request;
using APGDigitalIntegration.Common.CommonViewModels.Response;
using APGDigitalIntegration.Constant;
using APGDigitalIntegration.Domain.Interfaces;
using APGDigitalIntegration.Domain.Models;
using APGDigitalIntegration.DomainHelper.ExtensionMethods;
using APGDigitalIntegration.IAL.External.Interfaces.ICBOHosts;
using APGDigitalIntegration.IAL.External.Mpcss.Hosts.SimulateAdapter;
using APGDigitalIntegration.IAL.External.Mpcss.Interfaces;
using APGDigitalIntegration.IAL.Internal.Interfaces.APGFundamental;
using APGDigitalIntegration.IAL.Internal.Interfaces.APGTransaction;
using APGDigitalIntegration.IAL.Internal.Services.APGFundamental;
using APGDigitalIntegration.IAL.Internal.ViewModels.APGTransaction;
using APGExecutions.IAL.Internal.Interfaces.APGFundamentals;
using APGFundamentals.Application.Helper;
using APGMPCSSIntegration.Common.CommomMethods.MessageBuilders;
using APGMPCSSIntegration.Common.CommonViewModels.Request;
using APGMPCSSIntegration.Constant;
using APGMPCSSIntegration.DomainHelper;
using APGMPCSSIntegration.IAL.Internal.Communicator;
using Microsoft.Extensions.Localization;
using Spring.Globalization;
using CustomerNameVerificationRequest = APGDigitalIntegration.Common.CommonViewModels.Request.CustomerNameVerificationRequest;
using Envelope = APGDigitalIntegration.Common.CommonViewModels.Common.Envelope;

namespace APGDigitalIntegration.IAL.External.Mpcss.Hosts.CBOHostAdapters.Operational
{
    public class RecordVerificationHostAdapter : IRecordVerificationHostAdapter
    {
        #region Fields


        private readonly IMpcssCommunicator _mpcssCommunicator;
        private readonly IConfParamHelperService _confParamHelperService;
        private readonly IMessageQueue _messageQueue;
        private readonly ResponseMessageHandler _messageHandler;
        private readonly ISimulateHostAdapter _simulateHostAdapter;
        private readonly IMerchantMPCSSTransactionRequestsRepository _merchantMPCSSTransactionRequestsRepository;
        private readonly IMPCSSCommunicationLogService _mpcssCommunicationLogService;
        private readonly IMerchantAppService _merchantAppService;
        private readonly IMerchantOrderApiService _merchantOrderApiService;
        private readonly IMPCSSMessageBuilder _mpcssMessageBuilder;
        private readonly ICacheService _cacheService;
        private readonly IManualResetProvider _manualResetProvider;
        private readonly IStringLocalizer<RecordVerificationHostAdapter> _Localizer;
        private readonly IDateTimeProvider _dateTimeProvider;


        #endregion

        #region Constructor 
        public RecordVerificationHostAdapter(
            IMpcssCommunicator mpcssCommunicator,
            IConfParamHelperService confParamHelperService,
            ResponseMessageHandler messageHandler,
            ISimulateHostAdapter simulateHostAdapter,
            IMPCSSCommunicationLogService mpcssCommunicationLogService,
            IMerchantAppService merchantAppService,
            IMerchantOrderApiService merchantOrderApiService,
            IMPCSSMessageBuilder mpcssMessageBuilder,
            IMerchantMPCSSTransactionRequestsRepository merchantMPCSSTransactionRequestsRepository,
            ICacheService cacheService,
            IManualResetProvider manualResetProvider,
            IStringLocalizer<RecordVerificationHostAdapter> localizer,
            IDateTimeProvider dateTimeProvider)
        {
            _mpcssCommunicator = mpcssCommunicator;
            _messageHandler = messageHandler;
            _simulateHostAdapter = simulateHostAdapter;
            _mpcssCommunicationLogService = mpcssCommunicationLogService;
            _merchantAppService = merchantAppService;
            _merchantOrderApiService = merchantOrderApiService;
            _mpcssMessageBuilder = mpcssMessageBuilder;
            _merchantMPCSSTransactionRequestsRepository = merchantMPCSSTransactionRequestsRepository;
            _confParamHelperService = confParamHelperService;
            _simulateHostAdapter = simulateHostAdapter;
            _cacheService = cacheService;
            _manualResetProvider = manualResetProvider;
            _Localizer = localizer;
            _dateTimeProvider=dateTimeProvider;
        }

        #endregion

        #region Public Methods

        public async Task<ServiceResponse> Execute(CustomerNameVerificationRequest baseInternalRequest)
        {
            var mpcssMessage = MpcssMethods.PopulateMessageType(MPCSSRecordRequest.CustomerNameVerificationRequest);

            var participantPrefix = await _confParamHelperService.GetValue<string>(ConfigParam.MPCSSPSPRouteCode);

            var now = await _dateTimeProvider.NowByBankId(baseInternalRequest.BankId);
            var request = await _merchantMPCSSTransactionRequestsRepository.Add(new MerchantMPCSSTransactionRequest()
            {
                QROrderId = null,
                UniqueNotificationId = baseInternalRequest.UniqueNotificationId,
                Status = MPCSSStatus.Initiated.ToString(),
                TransactionType = MPCSSRecordRequest.CustomerNameVerificationRequest.ToString(),
                ParticipantPrefix = participantPrefix,
                RequestSourceId = baseInternalRequest.RequestSource,
                CreationDate=now,
                Language = Thread.CurrentThread.CurrentCulture.ToString(),
            });
            await _merchantMPCSSTransactionRequestsRepository.UnitOfWork.Commit();
            var msgIdentificationCode = request.MessageId;

            var externalRequest = await ConvertToExternalRequest(baseInternalRequest, mpcssMessage, msgIdentificationCode);
            var externalResponse = await SendMessage(externalRequest, mpcssMessage.QueueType, msgIdentificationCode, request.RequestSourceId).ConfigureAwait(false);

            _mpcssCommunicationLogService.SetMsgId(msgIdentificationCode);
            _mpcssCommunicationLogService.SetExternalRequest(externalRequest);
            await _mpcssCommunicationLogService.SetExternalRequestTime();

            return ConvertToInternalResponse(externalResponse);
        }

        #endregion

        #region Private Methods

        private async Task<Envelope> ConvertToExternalRequest(CustomerNameVerificationRequest mpcssRequest, MessageRequesitesDto messageRequesites, string msgIdentificationCode)
        {

            var mpcssParticipentShortName = await _confParamHelperService.GetValue<string>(ConfigParam.MPCSSPSPRouteCode);
            var merchantMpcssAccountData = await _merchantAppService.GetMPCSSAccountPaymentDataModel(mpcssRequest.MerchantRefId);

            var isoDateTime = DateTime.UtcNow.ToISODateTime();
            var customerNameVerification = new CustomerNameVerificationExternalRequest
            {
                GrpHdr = new GroupHeader()
                {
                    MsgId = msgIdentificationCode,
                    CreatedDateTime = isoDateTime
                },
                PrtcpntId = mpcssParticipentShortName,
                InstrMblOrSvc = merchantMpcssAccountData.MobileNumber,
                InstrAlias = null, // always use mobile number
                MblOrSvc = string.IsNullOrEmpty(mpcssRequest.MobileNumber) ? null : mpcssRequest.MobileNumber,
                Alias = string.IsNullOrEmpty(mpcssRequest.Alias) ? null : mpcssRequest.Alias
            };
            var request = new CustomerNameVerificationRoot
            {
                CustomerNameVerificationExternalRequestRequest = customerNameVerification
            };
            var message = _mpcssMessageBuilder.ConvertToExternalRequest(request, isoDateTime, msgIdentificationCode, messageRequesites.MessageType, false);
            return message;
        }
        private async Task<ServiceResponse> SendMessage(Envelope message, string queue, string messageId, int requestSourceId)
        {
            // Read this key from ConfParams.
            var isSimulated = await _confParamHelperService.GetValue<string>(ConfigParam.SimulateMPCSSTransaction).ConfigureAwait(false);

            ServiceResponse data = new ServiceResponse(success: true, responseCode: ResponseCodes.Success, message: _messageHandler.GetMessage(PaymentSuccessMessage.RequestSentSuccessfully));
            // Simulated Environment

            if (isSimulated.ToLower() == "true")
            {
                data = ConstructSimulatedResponse(message, messageId);
            }
            else
            {
                await _mpcssCommunicator.SendMessage(message, queue, ActiveMQMessageTypes.Text);
            }

            if (requestSourceId is (int)RequestSources.MerchantApp or (int)RequestSources.MerchantAppSDK)
            {
                BaseResponse<CustomerNameVerificationInternalResponse> response = null;
                var resetEvent = new ManualResetEvent(false);

                var mpcssVerificationTimeOutSeconds = await _confParamHelperService.GetValue<int>(ConfigParam.MPCSSVerificationTimeoutInSeconds);

                _manualResetProvider.AddManualReset(messageId, resetEvent);
                await _cacheService.GetSubScribe(messageId, (message) =>
                 {
                     response = JsonSerializer.Deserialize<BaseResponse<CustomerNameVerificationInternalResponse>>(message.ToString());

                     _manualResetProvider.FinishManualReset(messageId);
                 });


                int waitResult = WaitHandle.WaitAny(new[] { resetEvent }, (int)TimeSpan.FromSeconds(mpcssVerificationTimeOutSeconds).TotalMilliseconds);

                return new ServiceResponse(success: response != null ? response.Success : false,
                                           responseCode: response?.ResponseCode,
                                           message: response != null ? $"{response?.Message} {response?.ErrorList?.FirstOrDefault()}" : _Localizer[ResponseMessages.ServiceUnavailablePleaseTryAgainLater].Value,
                                           response?.Data, response?.ErrorList);

            }

            return data;

        }
        private ServiceResponse ConvertToInternalResponse(ServiceResponse externalResponse) => externalResponse;

        private ServiceResponse ConstructSimulatedResponse(Envelope request, string messageId)
        {
            #region Fields
            var requestMobileNoOrAlias = string.Empty;
            string status = "ACPT", reasonCode = "1000", customerName = "ABD#####@ampy", customerType = "PER", narration = string.Empty;
            var requestContent = Convert.ToString(request.Content.Value);

            if (requestContent.Contains("<MblOrSvc>"))
                requestMobileNoOrAlias = CommonMethods.GetXMLAttributeValue(XElement.Parse(requestContent), "MblOrSvc")?.Replace(" ", "");
            if (requestContent.Contains("<Alias>"))
                requestMobileNoOrAlias = CommonMethods.GetXMLAttributeValue(XElement.Parse(requestContent), "Alias");


            if (string.IsNullOrEmpty(requestMobileNoOrAlias))
                requestMobileNoOrAlias = CommonMethods.GetXMLAttributeValue(XElement.Parse(requestContent), "Alias");

            #endregion

            #region Error Mapping
            if ((requestContent.Contains("<MblOrSvc>") && !requestMobileNoOrAlias.Equals("23123456")) ||
                (requestContent.Contains("<Alias>") && !requestMobileNoOrAlias.Equals("Abdallah")))
            {
                status = "RJCT";
                reasonCode = "1";
                narration = "No customer was found with the given number or alias";
                customerName = string.Empty;
                customerType = string.Empty;

            }
            #endregion

            #region XML Construction
            var nameVerification = new CustomerNameVerificationExternalResponse()
            {
                GroupHeader = new GroupHeader()
                {
                    MsgId = "03ee19k3jxnfdct1",
                    CreatedDateTime = DateTime.UtcNow.ToISODateTime()
                },

                OrgnlMsgId = new OriginalMessageIdentifier()
                {
                    OriginalMessageId = messageId,
                    OriginalMessageType = "cstmrreg.20.01"
                },
                OrgnlMsgSts = new Common.CommonViewModels.Common.OriginalMessageIdentifiers.OriginalMessageStatus()
                {
                    Status = status,
                    ReasonCode = reasonCode,
                    Narration = narration

                },
                CustomerName = customerName,
                CustomerType = customerType
            };

            var nameVerificationResp = new CustomerNameVerificationResponseRoot
            {
                CustomerNameVerificationExternalResponse = nameVerification
            };

            var datetime = nameVerification.GroupHeader.CreatedDateTime;
            var message = _mpcssMessageBuilder.ConvertToExternalRequest(nameVerificationResp, datetime, nameVerification.OrgnlMsgId.OriginalMessageId, nameVerification.OrgnlMsgId.OriginalMessageType, false);

            #endregion

            #region Active Mq for Receiver
            Task.Run(() =>
            {
                Thread.Sleep(1000);
                _mpcssCommunicator.SendMessage(message, MPCSSQueues.CustomerNameResponseQueue, ActiveMQMessageTypes.Text);
            });
            #endregion

            return new ServiceResponse(success: true, responseCode: ResponseCodes.Success, message: _messageHandler.GetMessage(PaymentSuccessMessage.RequestSentSuccessfully));
        }


        #endregion
    }
}
