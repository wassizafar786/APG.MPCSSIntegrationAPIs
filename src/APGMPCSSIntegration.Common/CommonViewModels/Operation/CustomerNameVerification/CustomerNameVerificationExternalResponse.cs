using System.Xml.Serialization;
using APGDigitalIntegration.Common.CommonViewModels.Common.OriginalMessageIdentifiers;
using APGDigitalIntegration.Common.CommonViewModels.Payment_New;

namespace APGDigitalIntegration.Common.CommonViewModels.Registeration_New.CustomerNameVerification;



[XmlRoot(ElementName = "Document", Namespace = "urn:ats:mpc:CSTMRREG.21.01")]
public class CustomerNameVerificationResponseRoot
{
    [XmlElement(ElementName = "CstmrNameRes")]
    public CustomerNameVerificationExternalResponse CustomerNameVerificationExternalResponse { get; set; }
}



[XmlRoot("CstmrNameRes")]
public class CustomerNameVerificationExternalResponse
{
    public GroupHeader GroupHeader { get; set; }
    public OriginalMessageIdentifier OrgnlMsgId { get; set; }
    public OriginalMessageStatus OrgnlMsgSts { get; set; }
    
    [XmlElement("CstmrNm")]
    public string CustomerName { get; set; }
    
    [XmlElement("CstmrTp")]
    public string CustomerType { get; set; }
    
    public bool IsVerificationSuccess() => OrgnlMsgSts.Status == "ACPT";
}
