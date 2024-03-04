using System.Xml.Serialization;
using APGDigitalIntegration.Common.CommonViewModels.Common.OriginalMessageIdentifiers;
using APGDigitalIntegration.Common.CommonViewModels.Payment_New;
using APGMPCSSIntegration.Common.CommonViewModels.Response;

namespace APGDigitalIntegration.Common.CommonViewModels.Registeration_New;

[XmlRoot(ElementName = "Document", Namespace = "urn:ats:mpc:CSTMRREG.25.01")]
public class IsDefAcctReqRoot
{
    [XmlElement(ElementName = "CstmrNameReq")]
    public CustomerNameVerificationExternalRequest CustomerNameVerificationExternalRequest { get; set; }
}


[XmlRoot(ElementName = "Document", Namespace = "urn:ats:mpc:CSTMRREG.26.01")]
public class IsDefAcctReqResponseRoot
{
    public DefaultAccountVerificationExternalResponse DefaultAccountVerificationExternalResponse { get; set; }
}


[XmlRoot("IsDefAcctRes")]
public class DefaultAccountVerificationExternalResponse
{
    public GroupHeader GrpHdr { get; set; }
    public OriginalMessageIdentifier OrgnlMsgId { get; set; }
    public OriginalMessageStatus OrgnlMsgSts { get; set; }
    [XmlElement("Rply")]
    public string DefaultAccountReply { get; set; }
    [XmlElement("RgstrtnCd")]
    public string DefaultAccountRegistartionCode { get; set; }
}
