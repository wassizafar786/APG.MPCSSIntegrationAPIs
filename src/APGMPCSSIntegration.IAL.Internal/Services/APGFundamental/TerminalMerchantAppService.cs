using APGDigitalIntegration.IAL.Internal.Communicator;
using APGDigitalIntegration.IAL.Internal.Interfaces.APGFundamental;
using APGMPCSSIntegration.Constant;
using APGMPCSSIntegration.IAL.Internal.Communicator;
using APGDigitalIntegration.IAL.Internal.Viewmodel.APGFundamentals.Terminal;
using APGDigitalIntegration.IAL.Internal.ViewModels.APGFundamental.Terminal;
using APGMPCSSIntegration.DomainHelper.Exceptions;
using APGMPCSSIntegration.IAL.Internal.BaseRequests;

namespace APGDigitalIntegration.IAL.Internal.Services.APGFundamentals
{
    public class TerminalMerchantAppService : ITerminalMerchantAppService
    {
        private readonly IApiCaller _apiCaller;

        public TerminalMerchantAppService(IApiCaller apiCaller)
        {
            _apiCaller = apiCaller;
        }

        public async Task<BaseResponse<TerminalViewModel>> GetByTerminalId(long terminalId)
        {
            var queryParams = new Dictionary<string, string>()
            {
                {"terminalId", terminalId.ToString()}
            };
            return await _apiCaller.GetAsJson<TerminalViewModel>(MicroServicesURL.BaseFundamentalsURL,
                                                ControllerNames.Fundamentals.Terminal,
                                                ServiceNameLookup.GetByTerminalId,
                                                queryParams);
        }
        
        // TerminalMerchantCheck For Inward Transactions
        public async Task<BaseResponse<CheckTerminalMerchantResponse>> IsTerminalMerchantValid(CheckTerminalMerchantRequest checkTerminalMerchantRequest)
        {
            return await _apiCaller.PostAsJson<CheckTerminalMerchantResponse>(MicroServicesURL.BaseFundamentalsURL, 
                ServiceName.Fundamentals.ServiceNameTerminal, 
                ServiceNameLookup.IsTerminalMerchantValid, 
                checkTerminalMerchantRequest).ConfigureAwait(false);
        }

        public async Task<MerchantTerminalTransactionDataModel> GetMerchantTerminalTransactionData(long merchantRefId, long terminalNodeId)
        {
            var queryParams = new Dictionary<string, string>()
            {
                { "merchantRefId", merchantRefId.ToString() },
                { "terminalNodeId", terminalNodeId.ToString()}
            };
            
            var response = await _apiCaller.GetAsJson<MerchantTerminalTransactionDataModel>(
                MicroServicesURL.BaseFundamentalsURL,
                ServiceName.Fundamentals.ServiceNameTerminal,
                ServiceNameLookup.GetMerchantTerminalTransactionData,
                    queryParams);

            if (response.Success == false)
                throw new BusinessException(response.Message, response.ResponseCode);

            return response.Data;
        }
    }
}