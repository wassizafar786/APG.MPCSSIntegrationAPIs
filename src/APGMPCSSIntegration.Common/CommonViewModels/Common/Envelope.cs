using System.Xml;
using System.Xml.Serialization;

namespace APGDigitalIntegration.Common.CommonViewModels.Common;


[XmlType(AnonymousType = true, Namespace = "urn:ats:mpc:envelope")]
[XmlRoot(ElementName = "envelope", Namespace = "urn:ats:mpc:envelope")]
public class Envelope
{
    [XmlElement(ElementName = "id")]
    public string Id { get; set; }

    [XmlElement(ElementName = "type")]
    public string Type { get; set; }

    [XmlElement(ElementName = "format")]
    public string Format { get; set; }

    [XmlElement(ElementName = "date")]
    public string Date { get; set; }

    [XmlElement(ElementName = "signature")]
    public string Signature { get; set; }

    [XmlElement(ElementName = "content")]
    public XmlCDataSection Content { get; set; }

    [XmlIgnore]
    public string ContentData { get; set; }

}

[XmlType("OrgnlGrpInfAndSts")]
public class OriginalGroupStatusAndInformation
{
    [XmlElement("OrgnlMsgId")]
    public string OriginalMessageId { get; set; }

    [XmlElement("OrgnlMsgNmId")]
    public string OriginalMessageStatus { get; set; }

    [XmlElement("OrgnlCreDtTm")]
    public string OriginalMessageCreatedDate { get; set; }

    [XmlElement("GrpSts")]
    public string GroupStatus { get; set; }

    public StatusReasonInformation StsRsnInf { get; set; }
}

[XmlType("StsRsnInf")]
public class StatusReasonInformation
{
    [XmlElement("Rsn")]
    public Reason Reason { get; set; }

    [XmlElement("AddtlInf")]
    public string AdditionalInformation { get; set; }
}

public class Reason
{
    [XmlElement("Prtry")]
    public string OriginalGroupStatusProprietary { get; set; }
}
    