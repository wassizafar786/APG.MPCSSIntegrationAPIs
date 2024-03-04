using System.Xml.Serialization;
using APGDigitalIntegration.Common.CommonViewModels.Registeration_New.Common;

namespace APGDigitalIntegration.Common.CommonViewModels.Registeration_New.AccountManagement;


[XmlRoot("Document", Namespace = "CSTMRREG.07.01")]
public class CustomerAccountMaintenanceExternalRequestRoot
{
    public CustomerAccountMaintenanceExternalRequest CustomerAccountMaintenanceExternalRequest { get; set; }
}


[XmlType("AcntMntncReq")]
public class CustomerAccountMaintenanceExternalRequest
{
    public MessageIdentifier MsgId { get; set; }
    public string PrtcpntId { get; set; }
    public CustomerIdentification CstmrId { get; set; }
    public AccountInformationDto AcntInfo { get; set; }
    public string AdtnlInf { get; set; }

}