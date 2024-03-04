using System.Xml.Serialization;
using APGDigitalIntegration.Common.CommonViewModels.Payment_New;

namespace APGDigitalIntegration.Common.CommonViewModels.Registeration_New;


[XmlRoot(ElementName = "Document", Namespace = "urn:ats:mpc:CSTMRREG.20.01")]
public class CustomerNameVerificationRoot
{
    [XmlElement(ElementName = "CstmrNameReq")]
    public CustomerNameVerificationExternalRequest CustomerNameVerificationExternalRequestRequest { get; set; }
}


[XmlType("CstmrNameReq")]
public class CustomerNameVerificationExternalRequest
{
    public GroupHeader GrpHdr { get; set; }
    public string PrtcpntId { get; set; }
    public string InstrMblOrSvc { get; set; }
    public string InstrAlias { get; set; }
    public string MblOrSvc { get; set; }
    public string Alias { get; set; }
    public string RgstrtnCd { get; set; }
    public string AcntTp { get; set; }
}