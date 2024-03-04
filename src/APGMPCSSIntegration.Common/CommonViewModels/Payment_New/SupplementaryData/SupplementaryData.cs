using System.Xml.Serialization;

namespace APGDigitalIntegration.Common.CommonViewModels.Payment_New.SupplementaryData;

[XmlType("SplmtryData")]
public class SupplementaryData
{
    [XmlElement(ElementName = "PlcAndNm")]
    public string PlcAndNm { get; set; }

    [XmlElement(ElementName = "Envlp", Namespace = "http://www.Progressoft.com/ACH")]
    public Envlp Envlp { get; set; }
}