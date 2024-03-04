using System.Xml.Serialization;
using APGDigitalIntegration.Common.CommonViewModels.Payment_New;
using APGDigitalIntegration.Common.CommonViewModels.Response;

namespace APGMPCSSIntegration.Common.CommonViewModels.Response
{
    [XmlRoot("FIToFIPmtStsRpt")]
    public class PaymentReportResponse
    {
        public GroupHeaderResponseDto GrpHdr { get; set; }
        public OriginalGroupStatusInformationResponseDto OrgnlGrpInfAndSts { get; set; }
        public SupplementaryDataResponseDto SplmtryData { get; set; }
        public bool IsPaymentSuccess() => OrgnlGrpInfAndSts.GroupStatus == "ACSP";
    }

    public class GroupHeaderResponseDto
    {
        [XmlElement("MsgId")]
        public string PaymentMessageId { get; set; }
        [XmlElement("CreatedDateTime")]
        public string CreatedDateTime { get; set; }
        
        [XmlElement("NumberOfTranasctions")]
        public int NumberOfTransactions { get; set; }
 
        [XmlElement("TotalInterbankSettelmentAmount")]
        public ActiveAmountAndCurrency TotalInterbankSettlementAmount { get; set; }
        
        [XmlElement("InterbankSettlementDate")]
        public DateTime InterbankSettlementDate { get; set; }
        
        public SettlementInformationResponseDto SttlmInf { get; set; }
        
        public PaymentTypeInformationResponseDto PmtTpInf { get; set; }
        public InstructingAgentResponseDto InstgAgt { get; set; }
        public InstructedAgentResponseDto InstdAgt { get; set; }
    }

    public class InstructingAgentResponseDto
    {
        public FinancialInstitutionIdentificationResponseDto FinInstnId { get; set; }
    }

    public class InstructedAgentResponseDto
    {
        public FinancialInstitutionIdentificationResponseDto FinInstnId { get; set; }
    }

    public class FinancialInstitutionIdentificationResponseDto
    {
        public string BICFI { get; set; }
    }
    public class OriginalGroupStatusInformationResponseDto
    {
        [XmlElement("OrgnlMsgId")]
        public string OriginalMessageID { get; set; }
        
        [XmlElement("OrgnlMsgNmId")]
        public string OriginalMessageStatus { get; set; }
        
        [XmlElement("OrgnlCreDtTm")]
        public string OriginalMessageCreatedDate { get; set; }
        
        [XmlElement("GrpSts")]
        public string GroupStatus { get; set; }
        public StatusInformationResponseDto StsRsnInf { get; set; }
    }

    public class StatusInformationResponseDto
    {
        [XmlElement("Rsn")] 
        public ReasonInformationResponseDto Reason { get; set; }

        [XmlElement("AddtlInf")]
        public string AdditionalInformation { get; set; }
    }

    public class ReasonInformationResponseDto
    {
        [XmlElement("Prtry")]
        public string OriginalGroupStatusProprietary { get; set; }
    }


    public class PaymentReportResponseDto
    {
        public PaymentReportResponse FIToFIPmtStsRpt { get; set; }
    }

}
