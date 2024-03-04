using System.Xml.Serialization;
using APGDigitalIntegration.Common.CommonViewModels.Common.OriginalMessageIdentifiers;

namespace APGDigitalIntegration.Common.CommonViewModels.Operation.Registration.Response;


[XmlRoot("Document", Namespace = "CSTMRREG.10.01")]
public class RegistrationResponseRoot
{
    public RegistrationResponse RegResp { get; set; }
}

[XmlType("RegResp")]
public class RegistrationResponse
{
    public MessageIdentification MsgId { get; set; }
    public OriginalMessageIdentifier OrgnlMsgId { get; set; }
    public OriginalMessageStatus OrgnlMsgSts { get; set; }
    public string CorrelationId { get; set; }

}
    


    
