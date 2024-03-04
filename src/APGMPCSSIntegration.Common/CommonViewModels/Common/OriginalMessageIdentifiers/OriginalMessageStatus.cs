using System.Xml.Serialization;

namespace APGDigitalIntegration.Common.CommonViewModels.Common.OriginalMessageIdentifiers;

public class OriginalMessageStatus
{
    [XmlElement("Sts")]
    public string Status { get; set; }
    
    [XmlElement("RsnCd")]
    public string ReasonCode { get; set; }
    
    [XmlElement("Nrtn")]
    public string Narration { get; set; }
}