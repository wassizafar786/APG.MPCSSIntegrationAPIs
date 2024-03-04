using APGDigitalIntegration.IAL.Internal.Viewmodel.APGFundamentals.Terminal;
using APGDigitalIntegration.IAL.Internal.ViewModels.APGFundamental.Terminal;
using APGMPCSSIntegration.IAL.Internal.Communicator;

namespace APGDigitalIntegration.IAL.Internal.Interfaces.APGFundamental
{
    public interface ITerminalMerchantAppService
    {
        Task<BaseResponse<TerminalViewModel>> GetByTerminalId(long terminalId);
        Task<BaseResponse<CheckTerminalMerchantResponse>> IsTerminalMerchantValid(CheckTerminalMerchantRequest checkTerminalMerchantRequest);
        Task<MerchantTerminalTransactionDataModel> GetMerchantTerminalTransactionData(long merchantRefId, long terminalNodeId);
    }
}