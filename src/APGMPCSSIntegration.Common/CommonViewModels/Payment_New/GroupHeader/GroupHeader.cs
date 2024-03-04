using System.Xml.Serialization;

namespace APGDigitalIntegration.Common.CommonViewModels.Payment_New;

[XmlType("GrpHdr")]
public class GroupHeader
{
    [XmlElement(ElementName = "MsgId")]
    public string MsgId { get; set; }

    [XmlElement(ElementName = "CreDtTm")]
    public string CreatedDateTime { get; set; }

    [XmlElement(ElementName = "NbOfTxs")]
    public string NumberOfTranasctions { get; set; }

    [XmlElement(ElementName = "TtlIntrBkSttlmAmt")]
    public TotalInterBankSettlementAmount TotalInterbankSettlementAmount { get; set; }

    [XmlElement(ElementName = "IntrBkSttlmDt")]
    public string InterbankSettlementDate { get; set; }

    [XmlElement(ElementName = "SttlmInf")]
    public SettlementInformation SettlementInformation { get; set; }

    [XmlElement(ElementName = "PmtTpInf")]
    public PaymentTypeInformation PaymentTypeInformation { get; set; }

    [XmlElement(ElementName = "InstgAgt")]
    public InstructingAgent InstructingAgent { get; set; }

    [XmlElement(ElementName = "InstdAgt")]
    public InstructedAgent InstructedAgent { get; set; }
}