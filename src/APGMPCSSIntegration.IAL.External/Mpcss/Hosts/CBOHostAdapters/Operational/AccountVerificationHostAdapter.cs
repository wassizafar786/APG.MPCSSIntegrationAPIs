using System.Text.RegularExpressions;
using APGDigitalIntegration.Common.CommonMethods;
using APGDigitalIntegration.IAL.External.Interfaces.ICBOHosts;
using APGExecutions.IAL.Internal.Interfaces.APGFundamentals;
using APGMPCSSIntegration.Common.CommomMethods.MessageBuilders;
using APGMPCSSIntegration.Common.CommonViewModels.Payment;
using APGMPCSSIntegration.Common.CommonViewModels.Registration;
using APGMPCSSIntegration.Common.CommonViewModels.Request;
using APGMPCSSIntegration.Common.CommonViewModels.Response;
using APGMPCSSIntegration.Constant;
using APGMPCSSIntegration.DomainHelper;
using System.Xml.Linq;
using APG.MessageQueue.Mpcss.ActiveMQTransport;
using APG.MessageQueue.Mpcss.Interfaces;
using APGDigitalIntegration.Common.CommonMethods.MessageBuilders;
using APGDigitalIntegration.Common.CommonViewModels.Common.OriginalMessageIdentifiers;
using APGDigitalIntegration.Common.CommonViewModels.Payment_New;
using APGDigitalIntegration.Common.CommonViewModels.Registeration_New;
using APGDigitalIntegration.Common.CommonViewModels.Response;
using APGDigitalIntegration.Constant;
using APGDigitalIntegration.DomainHelper.ExtensionMethods;
using APGDigitalIntegration.IAL.External.Mpcss.Communicators;
using APGDigitalIntegration.IAL.External.Mpcss.Interfaces;
using Envelope = APGDigitalIntegration.Common.CommonViewModels.Common.Envelope;

namespace APGDigitalIntegration.IAL.External.Hosts.CBOHosts
{
    public class AccountVerificationHostAdapter : IAccountVerificationHostAdapter
    {
        #region Fields

        private readonly IMpcssCommunicator _mpcssCommunicator;
        private readonly IConfParamHelperService _confParamHelperService;
        private readonly ResponseMessageHandler _messageHandler = null;
        private readonly IMPCSSMessageBuilder _mpcssMessageBuilder;

        #endregion

        #region Constructor 
        public AccountVerificationHostAdapter(IMpcssCommunicator mpcssCommunicator, IConfParamHelperService confParamHelperService, ResponseMessageHandler messageHandler, IMPCSSMessageBuilder mpcssMessageBuilder)
        {
            _mpcssCommunicator = mpcssCommunicator;
            _messageHandler = messageHandler;
            _mpcssMessageBuilder = mpcssMessageBuilder;
            _confParamHelperService = confParamHelperService;
        }

        #endregion

        #region Public Methods

        public async Task<ServiceResponse> Execute(DefaultAccountInputDto baseInternalRequest, MPCSSRecordRequest mpcssMessageType)
        {
            var MpcssMessage = MpcssMethods.PopulateMessageType(mpcssMessageType);
            string date = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss");

            var externalRequest = ConvertToExternalRequest(baseInternalRequest, mpcssMessageType, date, MpcssMessage);

            var externalResponse = await SendMessage(externalRequest, MpcssMessage.QueueType).ConfigureAwait(false);

            return ConvertToInternalResponse(externalResponse);
        }

        #endregion

        #region Private Methods

        private Envelope ConvertToExternalRequest(DefaultAccountInputDto mpcssRequest, MPCSSRecordRequest MessageType, string date, MessageRequesitesDto messageRequesites)
        {
            var CustomerNameVerrification = new CustomerNameVerificationExternalRequest()
            {
                GrpHdr = new GroupHeader()
                {
                    MsgId = mpcssRequest.MessageIdentificationCode,
                    CreatedDateTime = date
                },
                PrtcpntId = MPCSSQueues.ParticipantShortName,
                MblOrSvc = mpcssRequest.MobileNumber,
                RgstrtnCd = mpcssRequest.RegistrationCode,
                AcntTp = mpcssRequest.AccountType
            };
            var request = new IsDefAcctReqRoot
            {
                CustomerNameVerificationExternalRequest = CustomerNameVerrification
            };
            var externalMessage = _mpcssMessageBuilder.ConvertToExternalRequest(request, date, mpcssRequest.MessageIdentificationCode, messageRequesites.MessageType, false);

            return externalMessage;
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
                return ConstructSimulatedResponse(message);
            }
        }
        private ServiceResponse ConvertToInternalResponse(ServiceResponse externalResponse)
        {
            return new ServiceResponse(
                 success: true,
                 responseCode: ResponseCodes.Success,
                 message: _messageHandler.GetMessage(PaymentSuccessMessage.RequestSentSuccessfully));
        }
        private ServiceResponse ConstructSimulatedResponse(Envelope request)
        {
            #region Fields
            string status = "ACPT", reasonCode = "1000", defaultAccountReply = "Y", DefaultAccountRegistartionCode = "AWPY001", narration = String.Empty;
            string requestMobileNo = CommonMethods.GetXMLAttributeValue(XElement.Parse(Convert.ToString(request.Content.Value)), "MblOrSvc");
            #endregion

            #region Error Mapping
            if (requestMobileNo.Equals("8006"))
            {
                status = "RJCT";
                reasonCode = "1001";
                narration = "No account was found with the given account id";
                defaultAccountReply = string.Empty;
                DefaultAccountRegistartionCode = string.Empty;
            }
            #endregion

            #region XML Construction
            DefaultAccountVerificationExternalResponse accountVerification = new DefaultAccountVerificationExternalResponse()
            {
                GrpHdr = new GroupHeader
                {
                    MsgId = "gnbp1ug31bn3ciqc",
                    CreatedDateTime = DateTime.UtcNow.ToISODateTime()
                },

                OrgnlMsgId = new OriginalMessageIdentifier()
                {
                    OriginalMessageId = "AWPY02092020029",
                    OriginalMessageType = "cstmrreg.25.01"
                },
                OrgnlMsgSts = new OriginalMessageStatus()
                {
                    Status = status,
                    ReasonCode = reasonCode,
                    Narration = narration
                },
                DefaultAccountReply = defaultAccountReply,
                DefaultAccountRegistartionCode = DefaultAccountRegistartionCode
            };

            var accountVerificationResp = new IsDefAcctReqResponseRoot
            {
                DefaultAccountVerificationExternalResponse = accountVerification
            };

            string datetime = accountVerification.GrpHdr.CreatedDateTime;
            
            var externalMessage = _mpcssMessageBuilder.ConvertToExternalRequest(accountVerificationResp, datetime, accountVerification.OrgnlMsgId.OriginalMessageId, accountVerification.OrgnlMsgId.OriginalMessageType, false);

            #endregion

            #region Active mq for Receiver
            Task.Run(() => { 
                _mpcssCommunicator.SendMessage(externalMessage, MPCSSQueues.CheckDefaultResponseQueue, ActiveMQMessageTypes.Text);
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
