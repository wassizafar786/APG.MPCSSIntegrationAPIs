using System.Xml.Serialization;
using APGDigitalIntegration.Common.CommonViewModels.Common;

namespace APGDigitalIntegration.Common.CommonViewModels.Payment_New.PaymentMesssages;

[XmlRoot(ElementName = "Document", Namespace = "urn:iso:std:iso:20022:tech:xsd:pacs.003.001.05")]
public class MPCSSPaymentStatusReportRoot
{
    [XmlElement(ElementName = "FIToFIPmtStsRpt")]
    public MPCSSPaymentStatusReport MPCSSPaymentStatusReport { get; set; }
}


[XmlRoot("FIToFIPmtStsRpt")]
public class MPCSSPaymentStatusReport
{
    public GroupHeader GroupHeader { get; set; }
    public OriginalGroupStatusAndInformation OrgnlGrpInfAndSts { get; set; }

    public SupplementaryData.SupplementaryData SplmtryData { get; set; }

    public bool IsPaymentSuccess() => OrgnlGrpInfAndSts.GroupStatus == "ACSP";
}