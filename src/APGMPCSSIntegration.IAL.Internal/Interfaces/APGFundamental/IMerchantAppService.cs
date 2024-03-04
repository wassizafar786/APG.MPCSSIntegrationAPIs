using APGDigitalIntegration.IAL.Internal.Viewmodel.APGFundamentals.Merchant;
using APGDigitalIntegration.IAL.Internal.ViewModels.APGFundamental.Merchant;
using APGMPCSSIntegration.IAL.Internal.Communicator;

namespace APGDigitalIntegration.IAL.Internal.Interfaces.APGFundamental
{
    public interface IMerchantAppService
    {
        Task<BaseResponse<MerchantViewModel>> GetById(long id);
        Task<BaseResponse<MerchantViewModel>> GetByMerchantId(long merchantId);
        Task<BaseResponse<OmanNetCardAccountViewModel>> GetOmanNetCardAccountByMerchantRefId(long merchantId);
        Task<BaseResponse<long>> GetMerchantRefIdByMerchantIdAsync(long merchantId);
        Task<MPCSSAccountPaymentDataModel> GetMPCSSAccountPaymentDataModel(long merchantRefId);
    }
}