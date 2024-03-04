using APGMPCSSIntegration.IAL.Internal.Communicator;
using System.Threading.Tasks;
using APGDigitalIntegration.IAL.Internal.ViewModels.APGTransaction;

namespace APGDigitalIntegration.IAL.Internal.Interfaces.APGTransaction
{
    public interface IShadowBalanceAppService
    {
        Task<BaseResponse<object>> CheckShadowBalanceLimit(CheckShadowBalanceLimitReq checkShadowBalanceLimitReq);
    }
}