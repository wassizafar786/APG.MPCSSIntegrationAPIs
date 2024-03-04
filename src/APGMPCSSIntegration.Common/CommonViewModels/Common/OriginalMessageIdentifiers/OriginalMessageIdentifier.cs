using System.Xml.Serialization;

namespace APGDigitalIntegration.Common.CommonViewModels.Common.OriginalMessageIdentifiers;

public class OriginalMessageIdentifier
{
    [XmlElement("Id")]
    public string OriginalMessageId { get; set; }
    [XmlElement("MsgTp")]
    public string OriginalMessageType { get; set; }
}