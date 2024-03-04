using APGDigitalIntegration.Common.CommonMethods;
using APGDigitalIntegration.Common.CommonServices;
using APGExecutions.IAL.Internal.Interfaces.APGFundamentals;
using APGMPCSSIntegration.Common.CommomMethods.MessageBuilders;
using APGMPCSSIntegration.Common.CommonViewModels.Payment;
using APGMPCSSIntegration.Common.CommonViewModels.Request;
using APGMPCSSIntegration.Common.CommonViewModels.Response;
using APGMPCSSIntegration.Constant;
using APGMPCSSIntegration.DomainHelper;
using APGMPCSSIntegration.IAL.External.Interfaces.ICBOHosts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using APG.MessageQueue.Mpcss.ActiveMQTransport;
using APGDigitalIntegration.Common.CommonMethods.MessageBuilders;
using APGDigitalIntegration.Common.CommonViewModels.Response;
using APGDigitalIntegration.Constant;
using APGDigitalIntegration.DomainHelper.ExtensionMethods;
using APGDigitalIntegration.IAL.External.Mpcss.Interfaces;
using APGDigitalIntegration.DomainHelper;
using Microsoft.Extensions.Localization;

namespace APGMPCSSIntegration.IAL.External.Hosts.CBOHosts
{
    public class PaymentCreditOutwardHostAdapter : IPaymentCreditOutwardHostAdapter
    {
        #region Fields

        private readonly IMpcssCommunicator _mpcssCommunicator;
        private readonly IConfParamHelperService _confParamHelperService;
        private readonly ICommonTransactionalAppService _commonTransactionalAppService;
        private readonly ResponseMessageHandler _messageHandler = null;
        private readonly IStringLocalizer<PaymentCreditOutwardHostAdapter> _stringLocalizer;


        #endregion

        #region Constructor 
        public PaymentCreditOutwardHostAdapter(IMpcssCommunicator mpcssCommunicator,
            IConfParamHelperService confParamHelperService,
            ICommonTransactionalAppService commonTransactionalAppService,
            ResponseMessageHandler messageHandler,
            IStringLocalizer<PaymentCreditOutwardHostAdapter> stringLocalizer)
        {
            _mpcssCommunicator = mpcssCommunicator;
            _messageHandler = messageHandler;
            _confParamHelperService = confParamHelperService;
            _commonTransactionalAppService = commonTransactionalAppService;
            _stringLocalizer = stringLocalizer;
        }

        #endregion

        #region Public Methods

        public async Task<ServiceResponse> Execute(CreditDebitPaymentInputDto baseInternalRequest, MPCSSRecordRequest mpcssMessageType)
        {
            var MpcssMessage = MpcssMethods.PopulateMessageType(mpcssMessageType);

            var externalRequest = await ConvertToExternalRequest(baseInternalRequest);

            var externalResponse = await SendMessage(externalRequest, MpcssMessage.QueueType).ConfigureAwait(false);

            return ConvertToInternalResponse(externalResponse);
        }

        #endregion


        #region Private Methods

        private async Task<MqMessage> ConvertToExternalRequest(CreditDebitPaymentInputDto mpcssRequest)
        {
            return await _commonTransactionalAppService.ConstructCreditTransactionXML(mpcssRequest);

        }
        private async Task<ServiceResponse> SendMessage(MqMessage message, string queue)
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
        private ServiceResponse ConstructSimulatedResponse(MqMessage request)
        {
            #region Fields
            PaymentReportResponse response = null; string groupStatus = "ACSP"; StatusInformationResponseDto statusInformationResponseDto = null;
            string requestAmount = CommonMethods.GetXMLAttributeValue(XElement.Parse(Convert.ToString(request.Contents)), "TtlIntrBkSttlmAmt");
            string grpMerchId = CommonMethods.GetXMLAttributeValue(XElement.Parse(Convert.ToString(request.Contents)), "grpMerchId");
            string terminalId = CommonMethods.GetXMLAttributeValue(XElement.Parse(Convert.ToString(request.Contents)), "terminalId");
            string msgId = CommonMethods.GetXMLAttributeValue(XElement.Parse(Convert.ToString(request.Contents)), "MsgId");

            #endregion

            #region Error mapping

            if (requestAmount.Equals("9001"))
            {
                groupStatus = "RJCT";
                statusInformationResponseDto = new StatusInformationResponseDto()
                {
                    Reason = new ReasonInformationResponseDto()
                    {
                        OriginalGroupStatusProprietary = "1001"
                    },
                    AdditionalInformation = "Exceeds the balance limit"
                };
            }
            else if (requestAmount.Equals("9002"))
            {
                groupStatus = "RJCT";
                statusInformationResponseDto = new StatusInformationResponseDto()
                {
                    Reason = new ReasonInformationResponseDto()
                    {
                        OriginalGroupStatusProprietary = "1001"
                    },
                    AdditionalInformation = "Reply timeout reached"
                };
            }
            else if (requestAmount.Equals("9003"))
            {
                groupStatus = "RJCT";
                statusInformationResponseDto = new StatusInformationResponseDto()
                {
                    Reason = new ReasonInformationResponseDto()
                    {
                        OriginalGroupStatusProprietary = "1"
                    },
                    AdditionalInformation = _stringLocalizer[PaymentRejectionReason.InvalidAccount.ToString()]
                };
            }
            #endregion

            #region XML Construction
            response = new PaymentReportResponse()

            {
                GrpHdr = new GroupHeaderResponseDto()
                {
                    PaymentMessageId = "TST307092020002",
                    CreatedDateTime = DateTime.UtcNow.ToISODateTime(),
                    InstgAgt = new InstructingAgentResponseDto()
                    {
                        FinInstnId = new FinancialInstitutionIdentificationResponseDto()
                        {
                            BICFI = "TESTOMR3"
                        }
                    },
                    InstdAgt = new InstructedAgentResponseDto()
                    {
                        FinInstnId = new FinancialInstitutionIdentificationResponseDto()
                        {
                            BICFI = "AMPLOMRU"
                        }
                    }
                },
                OrgnlGrpInfAndSts = new OriginalGroupStatusInformationResponseDto()
                {
                    OriginalMessageID = msgId,
                    OriginalMessageStatus = "pacs.008.001.05",
                    GroupStatus = groupStatus,
                    StsRsnInf = statusInformationResponseDto
                },
                SplmtryData = new SupplementaryDataResponseDto()
                {
                    PlcAndNm = "ACHSupplementaryData",
                    Envlp = new EnvelopeResponseDto()
                    {
                        achSupplementaryData = new ACHSupplementaryDataResponseDto()
                        {
                            BatchSource = "20",
                            SessionSequence = "314",
                            //GroupMerchantId= grpMerchId,
                            //TerminalId = terminalId
                        }
                    }
                }
            };

            PaymentReportResponseDto paymentReportResponseDto = new PaymentReportResponseDto()
            {
                FIToFIPmtStsRpt = response
            };

            var xmlRequest = MpcssMethods.ConvertRequestToXMLString(paymentReportResponseDto);
            string datetime = response.GrpHdr.CreatedDateTime;
            MqMessage message = MessageBuilder.ConstructMqMessage(xmlRequest, response.OrgnlGrpInfAndSts.OriginalMessageID, response.OrgnlGrpInfAndSts.OriginalMessageStatus, datetime);
            #endregion

            #region Active mq for Receiver

            Task.Run(() =>
            {
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
