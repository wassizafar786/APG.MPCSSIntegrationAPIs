using System.Threading.Tasks;
using Apache.NMS;
using APG.MessageQueue.Contracts.MerchantMPCSSOperations;
using APGDigitalIntegration.Common.CommonViewModels.Operation.Registration.Response;
using APGMPCSSIntegration.Common.CommonViewModels.Request;
using APGMPCSSIntegration.Common.CommonViewModels.Response;

namespace APGDigitalIntegration.Application.Interfaces
{
    public interface IMerchantMPCSSOperationAppService
    {
        Task MerchantMPCSSOperationRequest(UpdateMpcssMerchant model);
        Task MerchantMPCSSOperationResponse(RegistrationResponse registrationResponse);
    }
}
