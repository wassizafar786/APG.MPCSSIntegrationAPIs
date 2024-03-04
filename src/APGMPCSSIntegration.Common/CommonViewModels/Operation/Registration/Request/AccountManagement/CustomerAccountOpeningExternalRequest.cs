using System.Xml.Serialization;
using APGDigitalIntegration.Common.CommonViewModels.Registeration_New.Common;

namespace APGDigitalIntegration.Common.CommonViewModels.Registeration_New.AccountManagement;


[XmlRoot("Document", Namespace = "CSTMRREG.06.01")]
public class CustomerAccountOpeningExternalRequestRoot
{
    public CustomerAccountOpeningExternalRequest CustomerAccountOpeningExternalRequest { get; set; }
}


[XmlType("AcntOpngReq")]
public class CustomerAccountOpeningExternalRequest
{
    public MessageIdentifier MsgId { get; set; }
    public string PrtcpntId { get; set; }
    public CustomerIdentification CstmrId { get; set; }
    public AccountInformationDto AcntInfo { get; set; }
    public string AdtnlInf { get; set; }

}
