using APGDigitalIntegration.Common.CommonViewModels.Operation.Registration.Response;
using APGDigitalIntegration.DomainHelper.ViewModels;
using APGMPCSSIntegration.Common.CommonViewModels.Response;
using APGMPCSSIntegration.DomainHelper;

namespace APGDigitalIntegration.IAL.External.Mpcss.Hosts.SimulateAdapter
{
    public interface ISimulateHostAdapter
    {
        Task Insert(SimulateLogViewModel log);
        void Search(SimulateLogViewModel log);
        Task SimulateLogResponse(SimulateLogViewModel log);
        Task<MqMessage> MPCSSRecordResponse(SimulateLogViewModel log);
        RegistrationResponse ConstrutRegistrationResponse(MqMessage resp);
    }
}
