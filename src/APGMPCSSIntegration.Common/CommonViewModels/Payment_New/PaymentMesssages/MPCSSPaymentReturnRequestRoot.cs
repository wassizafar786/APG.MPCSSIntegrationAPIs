using System.Xml.Serialization;
using APGDigitalIntegration.Common.CommonViewModels.Response;
using APGMPCSSIntegration.Common.CommonViewModels.Payment;

namespace APGDigitalIntegration.Common.CommonViewModels.Payment_New.PaymentMesssages;
using SupplementaryData;



[XmlRoot(ElementName = "Document", Namespace = "urn:iso:std:iso:20022:tech:xsd:pacs.004.001.05")]
public class MPCSSPaymentReturnRequestRoot
{
    public MPCSSPaymentReturnRequest MPCSSPaymentReturnRequest { get; set; }
}


[XmlType("PmtRtr")]
public class MPCSSPaymentReturnRequest
{
    public GroupHeader GrpHdr { get; set; }
    
    public RefundTransactionInformation RefundTransactionInformation { get; set; }

    public OriginalGroupInformation OriginalGroupInformation { get; set; }
    
    public SupplementaryData SplmtryData { get; set; }
}


[XmlType("TxInf")]
public class RefundTransactionInformation
{
    public string RtrId { get; set; }
    public string OrgnlTxId { get; set; }
    public ActiveAmountAndCurrency RtrdIntrBkSttlmAmt { get; set; }
    public ReturnReasonInformationDto RtrRsnInf { get; set; }
}
