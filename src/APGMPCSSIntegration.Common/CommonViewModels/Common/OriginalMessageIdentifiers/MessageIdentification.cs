using System.Xml.Serialization;

namespace APGDigitalIntegration.Common.CommonViewModels.Common.OriginalMessageIdentifiers;

public class MessageIdentification
{
    [XmlElement("Id")]
    public string Id { get; set; }

    [XmlElement("CreatedDateTime")]
    public DateTime CreatedDateTime { get; set; }
}