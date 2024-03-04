using System.Xml.Serialization;
using APGDigitalIntegration.Common.CommonViewModels.Registeration_New.Common;

namespace APGDigitalIntegration.Common.CommonViewModels.Registeration_New.AccountManagement;


[XmlRoot("Document", Namespace = "CSTMRREG.08.01")]
public class CustomerAccountClosingExternalRequestRoot
{
    public CustomerAccountClosingExternalRequest CustomerAccountClosingExternalRequest { get; set; }
}



[XmlType("AcntClsgReq")]
public class CustomerAccountClosingExternalRequest
{
    public MessageIdentifier MsgId { get; set; }
    public string PrtcpntId { get; set; }
    public AccountIdentifier AcntId { get; set; }
    public string AdtnlInf { get; set; }

}