using System.Xml.Serialization;

namespace APGDigitalIntegration.Common.CommonViewModels.Payment_New.PaymentMesssages;

[XmlRoot(ElementName = "Document", Namespace = "urn:iso:std:iso:20022:tech:xsd:pacs.003.001.05")]
public class MPCSSPaymentDebitRequest
{
    public FIToFICstmrDrctDbt FIToFICstmrDrctDbt { get; set; }
}


[XmlType("FIToFICstmrDrctDbt")]
public class FIToFICstmrDrctDbt
{
    [XmlElement(ElementName = "GroupHeader")]
    public GroupHeader GroupHeader { get; set; }

    [XmlElement(ElementName = "DrctDbtTxInf")]
    public DebitTransferTransactionInformation CreditTransferTransactionInformation { get; set; }

    [XmlElement(ElementName = "SplmtryData")]
    public SupplementaryData.SupplementaryData SupplementaryData { get; set; }
}