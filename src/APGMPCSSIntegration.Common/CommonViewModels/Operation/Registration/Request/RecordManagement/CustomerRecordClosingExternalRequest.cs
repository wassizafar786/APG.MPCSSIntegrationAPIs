using System.Xml.Serialization;
using APGDigitalIntegration.Common.CommonViewModels.Registeration_New.Common;

namespace APGDigitalIntegration.Common.CommonViewModels.Registeration_New.RecordManagement;


[XmlRoot("Document", Namespace = "CSTMRREG.03.01")]
public class CustomerRecordClosingExternalRequestRoot
{
    public CustomerRecordClosingExternalRequest CustomerRecordClosingExternalRequest { get; set; }
}


[XmlType("CstmrClsgReq")]
public class CustomerRecordClosingExternalRequest
{
    public MessageIdentifier MsgId { get; set; }
    public string PrtcpntId { get; set; }
    public CustomerIdentification CstmrId { get; set; }
    public string AdtnlInf { get; set; }
    public string CorrelationId { get; set; }

}

