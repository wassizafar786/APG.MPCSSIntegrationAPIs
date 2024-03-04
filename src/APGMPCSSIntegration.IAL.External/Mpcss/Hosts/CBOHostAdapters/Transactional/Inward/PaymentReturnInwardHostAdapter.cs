using APG.MessageQueue.Mpcss.ActiveMQTransport;
using APGExecutions.IAL.Internal.Interfaces.APGFundamentals;
using APGMPCSSIntegration.Common.CommomMethods.MessageBuilders;
using APGMPCSSIntegration.Common.CommonViewModels.Response;
using APGMPCSSIntegration.Constant;
using APGMPCSSIntegration.DomainHelper;
using APGDigitalIntegration.Common.CommonMethods.MessageBuilders;
using APGDigitalIntegration.Common.CommonViewModels.Response;
using APGDigitalIntegration.DomainHelper.ExtensionMethods;
using APGDigitalIntegration.IAL.External.Mpcss.Interfaces;
using APGDigitalIntegration.IAL.External.Mpcss.Interfaces.ICBOHostAdapters;

namespace APGDigitalIntegration.IAL.External.Mpcss.Hosts.CBOHostAdapters
{
    public class PaymentReturnInwardHostAdapter : IPaymentReturnInwardHostAdapter
    {
        #region Fields

        private readonly IMpcssCommunicator _mpcssCommunicator;
        private readonly IConfParamHelperService _confParamHelperService;
        private readonly ResponseMessageHandler _messageHandler = null;

        #endregion

        #region Constructor 
        public PaymentReturnInwardHostAdapter(IMpcssCommunicator mpcssCommunicator, IConfParamHelperService confParamHelperService, ResponseMessageHandler messageHandler)
        {
            _mpcssCommunicator = mpcssCommunicator;
            _messageHandler = messageHandler;
            _confParamHelperService = confParamHelperService;
        }

        #endregion

        #region Public Methods

        public async Task<ServiceResponse> Execute(PaymentReturnResponseDto baseInternalRequest, MPCSSRecordRequest mpcssMessageType)
        {
            var MpcssMessage = MpcssMethods.PopulateMessageType(mpcssMessageType);

            var externalRequest = ConvertToExternalRequest(baseInternalRequest, MpcssMessage);

            var externalResponse = await SendMessage(externalRequest, MpcssMessage.QueueType).ConfigureAwait(false);

            return ConvertToInternalResponse(externalResponse);
        }

        #endregion

        #region Private Methods

        private MqMessage ConvertToExternalRequest(PaymentReturnResponseDto request, MessageRequesitesDto messageRequesites)
        {
            string xmlRequest = string.Empty;
            DateTime date = DateTime.UtcNow;
            PaymentReportResponse PaymentResponse = new PaymentReportResponse();
            GroupHeaderResponseDto GrpHdr = new GroupHeaderResponseDto();
            GrpHdr.PaymentMessageId = "test"; // Todo later 
            GrpHdr.CreatedDateTime = DateTime.UtcNow.ToISODateTime();

            InstructingAgentResponseDto InstgAgt = new InstructingAgentResponseDto();
            FinancialInstitutionIdentificationResponseDto InstructingFinInstnId = new FinancialInstitutionIdentificationResponseDto();
            InstructingFinInstnId.BICFI = "test"; // Todo later 
            InstgAgt.FinInstnId = InstructingFinInstnId;
            GrpHdr.InstgAgt = InstgAgt;

            InstructedAgentResponseDto InstdAgt = new InstructedAgentResponseDto();
            FinancialInstitutionIdentificationResponseDto InstructedFinInstnId = new FinancialInstitutionIdentificationResponseDto();
            InstructedFinInstnId.BICFI = "test"; // Todo later
            InstdAgt.FinInstnId = InstructedFinInstnId;
            GrpHdr.InstdAgt = InstdAgt;

            OriginalGroupStatusInformationResponseDto OrgnlGrpInfAndSts = new OriginalGroupStatusInformationResponseDto();
            OrgnlGrpInfAndSts.OriginalMessageID = request.GrpHdr.PaymentMessageId;
            OrgnlGrpInfAndSts.OriginalMessageStatus = messageRequesites.MessageType;
            OrgnlGrpInfAndSts.GroupStatus = "ACSP";
            StatusInformationResponseDto StsRsnInf = new StatusInformationResponseDto();
            ReasonInformationResponseDto Rsn = new ReasonInformationResponseDto();
            Rsn.OriginalGroupStatusProprietary = "test"; // Todo later
            StsRsnInf.Reason = Rsn;
            OrgnlGrpInfAndSts.StsRsnInf = StsRsnInf;

            SupplementaryDataResponseDto SplmtryData = new SupplementaryDataResponseDto();
            SplmtryData.PlcAndNm = "ACHSupplementaryData";
            EnvelopeResponseDto Envlp = new EnvelopeResponseDto();
            ACHSupplementaryDataResponseDto achSupplementaryData = new ACHSupplementaryDataResponseDto();
            achSupplementaryData.SessionSequence = "test"; // todo later
            achSupplementaryData.BatchSource = "test"; // todo later
            Envlp.achSupplementaryData = achSupplementaryData;
            SplmtryData.Envlp = Envlp;
            PaymentResponse.GrpHdr = GrpHdr;
            PaymentResponse.OrgnlGrpInfAndSts = OrgnlGrpInfAndSts;
            PaymentResponse.SplmtryData = SplmtryData;

            PaymentReportResponseDto paymentReportResponseDto = new PaymentReportResponseDto()
            {
                FIToFIPmtStsRpt = PaymentResponse
            };

            var isSimulated = _confParamHelperService.GetValue<string>(ConfigParam.SimulateMPCSSTransaction, null).Result;

            if (!string.IsNullOrEmpty(isSimulated) && isSimulated == "true")
            {
                // Simulated Environment
                ConstructSimulatedResponse(Convert.ToString(request.TxInf.ReturnedInterBankSettlementAmount), ref paymentReportResponseDto);
            }

            xmlRequest = MpcssMethods.ConvertRequestToXMLString(paymentReportResponseDto);
            xmlRequest = MpcssMethods.BuildAchNs2Data(xmlRequest);
            MqMessage message = MessageBuilder.ConstructMqMessage(xmlRequest, request.GrpHdr.PaymentMessageId, messageRequesites.MessageType, date.ToString("yyyy-MM-ddTHH:mm:ss"));
            return message;
        }
        private async Task<ServiceResponse> SendMessage(MqMessage message, string queue)
        {
            // Read this key from ConfParams.
            var isSimulated = await _confParamHelperService.GetValue<string>(ConfigParam.SimulateMPCSSTransaction, null).ConfigureAwait(false);

            if (string.IsNullOrEmpty(isSimulated) || isSimulated != "true")
            {
                await _mpcssCommunicator.SendMessage(message, queue, ActiveMQMessageTypes.Bytes);

                return new ServiceResponse(
              success: true,
              responseCode: ResponseCodes.Success,
              message: _messageHandler.GetMessage(PaymentSuccessMessage.RequestSentSuccessfully));
            }
            //else
            //{
            //    // Simulated Environment
            //    return ConstructSimulatedResponse(message);
            //}
            return new ServiceResponse(
             success: true,
             responseCode: ResponseCodes.Success,
             message: _messageHandler.GetMessage(PaymentSuccessMessage.RequestSentSuccessfully));
        }
        private ServiceResponse ConvertToInternalResponse(ServiceResponse externalResponse)
        {
            return new ServiceResponse(
                 success: true,
                 responseCode: ResponseCodes.Success,
                 message: _messageHandler.GetMessage(PaymentSuccessMessage.RequestSentSuccessfully));
        }
        private void ConstructSimulatedResponse(string requestAmount,ref PaymentReportResponseDto response)
        {
            if (requestAmount.Equals("9002"))
            {
                response.FIToFIPmtStsRpt.OrgnlGrpInfAndSts.GroupStatus = "RJCT";
                response.FIToFIPmtStsRpt.OrgnlGrpInfAndSts.StsRsnInf = new StatusInformationResponseDto()
                {
                    Reason = new ReasonInformationResponseDto()
                    {
                        OriginalGroupStatusProprietary = "1105"
                    },
                    AdditionalInformation = "Reply timeout reached"
                };
            }

        }


        #endregion
    }
}
