using System.Xml.Serialization;

namespace APGDigitalIntegration.Common.CommonViewModels.Payment_New.SupplementaryData;

[XmlType(Namespace = "http://www.Progressoft.com/ACH")]
public class AchSupplementaryData
{
    [XmlElement(ElementName = "batchSource")]
    public string BatchSource { get; set; }

    [XmlElement(ElementName = "consumerId")]
    public string ConsumerId { get; set; }

    [XmlElement(ElementName = "merchCategoryCd")]
    public string MerchCategoryCd { get; set; }

    [XmlElement(ElementName = "grpMerchId")]
    public string GroupMerchantId { get; set; }

    [XmlElement(ElementName = "terminalId")]
    public string TerminalId { get; set; }

    [XmlElement(ElementName = "filler")]
    public string Filler { get; set; }

    [XmlElement(ElementName = "merchantName")]
    public string MerchantName { get; set; }

    [XmlElement(ElementName = "pntOfInitiateMethd")]
    public string PointOfInitiateMethod { get; set; }

    [XmlElement(ElementName = "msgtipOrConvnceIndctrId")]
    public string MsgtipOrConvnceIndctrId { get; set; }

    [XmlElement(ElementName = "feePercentage")]
    public string FeePercentage { get; set; }

    [XmlElement(ElementName = "countryCd")]
    public string CountryCd { get; set; }

    [XmlElement(ElementName = "merchantCity")]
    public string MerchantCity { get; set; }

    [XmlElement(ElementName = "postCd")]
    public string PostCd { get; set; }

    [XmlElement(ElementName = "invoiceNumber")]
    public string InvoiceNumber { get; set; }

    [XmlElement(ElementName = "sessionSequence")]
    public string SessionSequence { get; set; }

    [XmlElement(ElementName = "receiverIdIssuingCountry")]
    public string ReceiverIdIssuingCountry { get; set; }

    [XmlElement(ElementName = "receiverIdType")]
    public string ReceiverIdType { get; set; }

    [XmlElement(ElementName = "receiverIdValue")]
    public string ReceiverIdValue { get; set; }

    [XmlElement(ElementName = "receiverName")]
    public string ReceiverName { get; set; }
}