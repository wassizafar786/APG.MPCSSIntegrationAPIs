using System.Xml.Serialization;

namespace APGDigitalIntegration.Common.CommonViewModels.Payment_New.PaymentMesssages;

[XmlRoot(ElementName = "Document", Namespace = "urn:iso:std:iso:20022:tech:xsd:pacs.008.001.05")]
public class MPCSSPaymentCreditRequestRoot
{
    public MPCSSPaymentCreditRequest MPCSSPaymentCreditRequest { get; set; }
}


[XmlType("FIToFICstmrCdtTrf")]
public class MPCSSPaymentCreditRequest
{
    [XmlElement(ElementName = "GroupHeader")]
    public GroupHeader GroupHeader { get; set; }

    [XmlElement(ElementName = "CdtTrfTxInf")]
    public CreditTransferTransactionInformation CreditTransferTransactionInformation { get; set; }

    [XmlElement(ElementName = "SplmtryData")]
    public SupplementaryData.SupplementaryData SupplementaryData { get; set; }
}
