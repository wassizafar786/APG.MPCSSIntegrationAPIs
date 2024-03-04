using System.Xml.Serialization;

namespace APGDigitalIntegration.Common.CommonViewModels.Registeration_New.Common;

[XmlType("MsgId")]
public class MessageIdentifier
{
    public string Id { get; set; }
    public string CreDtTm { get; set; }
}