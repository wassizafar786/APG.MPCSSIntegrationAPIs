using System.Xml.Serialization;
using APGDigitalIntegration.Common.CommonViewModels.Registeration_New.Common;
using APGMPCSSIntegration.Common.CommonViewModels.Registration;

namespace APGDigitalIntegration.Common.CommonViewModels.Registeration_New.RecordManagement;

[XmlRoot(ElementName = "Document", Namespace = "urn:ats:mpc:CSTMRREG.02.01")]
public class CustomerRecordMaintenanceExternalRequestRoot
{
    public CustomerRecordMaintenanceExternalRequest CustomerRecordMaintenanceExternalRequest { get; set; }
}


[XmlType("CstmrMntncReq")]
public class CustomerRecordMaintenanceExternalRequest
{
    public MessageIdentifier MsgId { get; set; }
    public string PrtcpntId { get; set; }
    public CustomerInformation CstmrInfo { get; set; }
    public AddressInformation Address { get; set; }
    public string AdtnlInf { get; set; }
    public string CorrelationId { get; set; }
}