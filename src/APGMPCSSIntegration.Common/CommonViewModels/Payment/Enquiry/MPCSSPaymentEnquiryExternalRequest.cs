using System.Xml.Serialization;

namespace APGDigitalIntegration.Common.CommonViewModels.Payment.Enquiry;


[XmlRoot("FIToFIPmtStsReq")]
public class MPCSSPaymentEnquiryExternalRequest
{
    [XmlElement("GroupHeader")]
    public PaymentEnquiryGroupHeader GroupHeader { get; set; }
    
    [XmlElement("OrgnlGrpInf")]
    public EnquiryOriginalGroupInformation EnquiryOriginalGroupInformation { get; set; }
}

public class PaymentEnquiryGroupHeader
{
    [XmlElement("MsgId")]
    public string MessageIdentification { get; set; }
    
    [XmlElement("CreatedDateTime")]
    public string CreatedDateTime { get; set; }
}

public class EnquiryOriginalGroupInformation
{    
    [XmlElement("OrgnlMsgId")]
    public string OriginalMessageIdentification { get; set; }
   
    [XmlElement("OrgnlMsgNmId")]
    public string OriginalMessageNameIdentification { get; set; }
    
    [XmlElement("OrgnlCreDtTm>")]
    public string OriginalCreationDateAndTime { get; set; }
}