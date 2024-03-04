using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace APGMPCSSIntegration.Common.CommonViewModels.Response
{
    [XmlRoot("envelope")]
    public class Envelope
    {
        [XmlElement("id")]
        public string MessageId { get; set; }
        [XmlElement("type")]
        public string MessageType { get; set; }
        [XmlElement("format")]
        public string MessageFormat { get; set; }
        [XmlElement("date")]
        public string TransactionDate { get; set; }
        [XmlElement("signature")]
        public string DigitalSignature { get; set; }
        [XmlElement("content")]
        public string MessageContent { get; set; }
    }
    
}
