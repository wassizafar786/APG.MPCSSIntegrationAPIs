using APGDigitalIntegration.Common.CommonViewModels.Response;
using APGMPCSSIntegration.Common.CommonViewModels.Request;
using APGMPCSSIntegration.Common.CommonViewModels.Response;
using APGMPCSSIntegration.Constant;
using APGMPCSSIntegration.DomainHelper;

namespace APGDigitalIntegration.IAL.External.Interfaces.ICBOHosts
{
    public interface IRecordRegistrationHostAdapter
    {
        Task<ServiceResponse> Execute(RegistrationRecordInputDto baseInternalRequest, MPCSSRecordRequest mpcssMessageType);
    }
}
