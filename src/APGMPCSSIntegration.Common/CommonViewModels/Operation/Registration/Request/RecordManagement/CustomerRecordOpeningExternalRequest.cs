using System.Xml.Serialization;
using APGDigitalIntegration.Common.CommonViewModels.Registeration_New.Common;
using APGMPCSSIntegration.Common.CommonViewModels.Registration;

namespace APGDigitalIntegration.Common.CommonViewModels.Registeration_New.RecordManagement;

[XmlRoot(ElementName = "Document", Namespace = "urn:ats:mpc:CSTMRREG.01.01")]

public class CustomerRecordOpeningExternalRequestRoot
{
    public CustomerRecordOpeningExternalRequest CustomerRecordOpeningExternalRequest { get; set; }
}


[XmlType("CstmrOpngReq")]
public class CustomerRecordOpeningExternalRequest
{
    public MessageIdentifier MsgId { get; set; }
    public string PrtcpntId { get; set; }
    public CustomerInformation CstmrInfo { get; set; }
    public AddressInformation Address { get; set; }
    public string AdtnlInf { get; set; }
    public string CorrelationId { get; set; }
}