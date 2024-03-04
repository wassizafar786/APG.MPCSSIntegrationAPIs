using System.Xml.Serialization;

namespace APGDigitalIntegration.Common.CommonViewModels.Payment_New.SupplementaryData;

public class Envlp
{
    [XmlElement(ElementName = "achSupplementaryData", Namespace = "http://www.Progressoft.com/ACH")]
    public AchSupplementaryData achSupplementaryData { get; set; }
}