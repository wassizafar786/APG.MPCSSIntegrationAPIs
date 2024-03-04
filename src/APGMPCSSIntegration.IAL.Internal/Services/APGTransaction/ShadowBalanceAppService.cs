using System.Threading.Tasks;
using APGDigitalIntegration.IAL.Internal.Communicator;
using APGDigitalIntegration.IAL.Internal.Interfaces.APGTransaction;
using APGDigitalIntegration.IAL.Internal.Viewmodel;
using APGDigitalIntegration.IAL.Internal.ViewModels.APGTransaction;
using APGMPCSSIntegration.Constant;
using APGMPCSSIntegration.DomainHelper.Exceptions;
using APGMPCSSIntegration.IAL.Internal.Communicator;

namespace APGDigitalIntegration.IAL.Internal.Services.APGTransaction
{
    public class ShadowBalanceAppService : IShadowBalanceAppService
    {
        private readonly IApiCaller _apiCaller;

        public ShadowBalanceAppService(IApiCaller apiCaller)
        {
            _apiCaller = apiCaller;
        }

        public async Task<BaseResponse<object>> CheckShadowBalanceLimit(CheckShadowBalanceLimitReq checkShadowBalanceLimitReq)
        {
            var response = await _apiCaller.PostAsJson<object>(
                    MicroServicesURL.BaseTransactionsURL,
                    ControllerNames.Transactions.ShadowBalance,
                    ServiceName.Transactions.CheckShadowBalanceLimit,
                    checkShadowBalanceLimitReq)
                .ConfigureAwait(false);

            return response;
        }
    }
}