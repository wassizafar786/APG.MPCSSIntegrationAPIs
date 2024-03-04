using APG.MessageQueue.Mpcss.ActiveMQTransport;
using APGDigitalIntegration.Common.CommonMethods.MessageBuilders;
using APGDigitalIntegration.Common.CommonViewModels.Payment;
using APGDigitalIntegration.Common.CommonViewModels.Response;
using APGDigitalIntegration.IAL.External.Interfaces.ICBOHosts;
using APGDigitalIntegration.IAL.External.Mpcss.Communicators;
using APGDigitalIntegration.IAL.External.Mpcss.Interfaces;
using APGExecutions.IAL.Internal.Interfaces.APGFundamentals;
using APGMPCSSIntegration.Common.CommomMethods.MessageBuilders;
using APGMPCSSIntegration.Common.CommonViewModels.Payment;
using APGMPCSSIntegration.Common.CommonViewModels.Request;
using APGMPCSSIntegration.Constant;
using APGMPCSSIntegration.DomainHelper;
using Envelope = APGDigitalIntegration.Common.CommonViewModels.Payment.Envelope;

namespace APGDigitalIntegration.IAL.External.Mpcss.Hosts.CBOHostAdapters.Transactional
{
    public class PaymentStatusReportHostAdapter : IPaymentStatusReportHostAdapter
    {
        #region Fields

        private readonly IMpcssCommunicator _mpcssCommunicator;
        private readonly IConfParamHelperService _confParamHelperService;
        private readonly ResponseMessageHandler _messageHandler = null;

        #endregion

        #region Constructor 
        public PaymentStatusReportHostAdapter(IMpcssCommunicator mpcssCommunicator, IConfParamHelperService confParamHelperService, ResponseMessageHandler messageHandler)
        {
            _mpcssCommunicator = mpcssCommunicator;
            _messageHandler = messageHandler;
            _confParamHelperService = confParamHelperService;
        }

        #endregion

        #region Public Methods

        public async Task<ServiceResponse> Execute(PaymentStatusReportInputDto baseInternalRequest, MPCSSRecordRequest mpcssMessageType)
        {
            var MpcssMessage = MpcssMethods.PopulateMessageType(mpcssMessageType);

            var externalRequest = ConvertToExternalRequest(baseInternalRequest, mpcssMessageType, MpcssMessage);

            var externalResponse = await SendMessage(externalRequest, MpcssMessage.QueueType).ConfigureAwait(false);

            return ConvertToInternalResponse(externalResponse);
        }

        #endregion

        #region Private Methods

        private MqMessage ConvertToExternalRequest(PaymentStatusReportInputDto mpcssRequest, MPCSSRecordRequest MessageType, MessageRequesitesDto messageRequesites)
        {
            string xmlRequest = string.Empty;
            string date = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss");
            PaymentStatusReportDto PaymentResponse = new PaymentStatusReportDto();
            GroupHeaderDto GrpHdr = new GroupHeaderDto();
            GrpHdr.MsgId = mpcssRequest.MessageIdentificationCode;
            GrpHdr.CreDtTm = date;
            InstructingAgent InstgAgt = null;
            if (mpcssRequest.InstructingAgentBICFI != null)
            {
                InstgAgt = new InstructingAgent();
                FinancialInstitutionIdentification InstructingFinInstnId = new FinancialInstitutionIdentification();
                InstructingFinInstnId.BICFI = mpcssRequest.InstructingAgentBICFI;
                InstgAgt.FinInstnId = InstructingFinInstnId;
            }
            GrpHdr.InstgAgt = InstgAgt;
            InstructedAgent InstdAgt = null;
            if (mpcssRequest.InstructedAgentBICFI != null)
            {
                InstdAgt = new InstructedAgent();
                FinancialInstitutionIdentification InstructedFinInstnId = new FinancialInstitutionIdentification();
                InstructedFinInstnId.BICFI = mpcssRequest.InstructedAgentBICFI;
                InstdAgt.FinInstnId = InstructedFinInstnId;
            }
            GrpHdr.InstdAgt = InstdAgt;

            OriginalGroupStatusInformationDto OrgnlGrpInfAndSts = new OriginalGroupStatusInformationDto();
            OrgnlGrpInfAndSts.OrgnlMsgId = mpcssRequest.OriginalMessageId;
            OrgnlGrpInfAndSts.OrgnlMsgNmId = mpcssRequest.OriginalMessageNameId;
            OrgnlGrpInfAndSts.GrpSts = mpcssRequest.GroupStatus;
            StatusInformationDto StsRsnInf = null;
            if (mpcssRequest.OriginalGroupStatusProprietary != null)
            {
                StsRsnInf = new StatusInformationDto();
                ReasonInformationDto Rsn = new ReasonInformationDto();
                Rsn.Prtry = mpcssRequest.OriginalGroupStatusProprietary;
                StsRsnInf.Rsn = Rsn;
            }
            OrgnlGrpInfAndSts.StsRsnInf = StsRsnInf;
            SupplementaryDataDto SplmtryData = new SupplementaryDataDto();
            SplmtryData.PlcAndNm = "ACHSupplementaryData";
            Envelope Envlp = new Envelope();
            ACHSupplementaryData achSupplementaryData = new ACHSupplementaryData();
            achSupplementaryData.receiverIdIssuingCountry = mpcssRequest.ReceiverIdIssuingCountry;
            achSupplementaryData.receiverIdType = mpcssRequest.ReceiverIdType;
            achSupplementaryData.receiverIdValue = mpcssRequest.ReceiverIdValue;
            achSupplementaryData.receiverName = mpcssRequest.SupplementaryReceiverName;
            achSupplementaryData.sessionSequence = mpcssRequest.SessionSequence;
            achSupplementaryData.batchSource = mpcssRequest.BatchSource;
            Envlp.achSupplementaryData = achSupplementaryData;
            SplmtryData.Envlp = Envlp;
            PaymentResponse.GrpHdr = GrpHdr;
            PaymentResponse.OrgnlGrpInfAndSts = OrgnlGrpInfAndSts;
            PaymentResponse.SplmtryData = SplmtryData;

            var request = new PaymentRequestDto
            {
                FIToFIPmtStsRpt = PaymentResponse
            };
            xmlRequest = MpcssMethods.ConvertRequestToXMLString(request);
            xmlRequest = MpcssMethods.BuildAchNs2Data(xmlRequest);
            MqMessage message = MessageBuilder.ConstructMqMessage(xmlRequest, mpcssRequest.MessageIdentificationCode, messageRequesites.MessageType, date);
            return message;
           
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
            return new ServiceResponse(
         success: true,
         responseCode: ResponseCodes.Success,
         message: _messageHandler.GetMessage(PaymentSuccessMessage.RequestSentSuccessfully));

        }


        #endregion
    }
}
