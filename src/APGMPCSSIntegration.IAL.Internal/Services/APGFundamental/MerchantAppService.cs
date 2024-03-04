using System.Collections.Generic;
using System.Threading.Tasks;
using APGDigitalIntegration.IAL.Internal.Communicator;
using APGDigitalIntegration.IAL.Internal.Interfaces.APGFundamental;
using APGDigitalIntegration.IAL.Internal.Viewmodel.APGFundamentals.Merchant;
using APGDigitalIntegration.IAL.Internal.ViewModels.APGFundamental.Merchant;
using APGMPCSSIntegration.Constant;
using APGMPCSSIntegration.DomainHelper.Exceptions;
using APGMPCSSIntegration.IAL.Internal.Communicator;

namespace APGExecutions.IAL.Internal.Services.APGFundamentals
{
    public class MerchantAppService : IMerchantAppService
    {
        private readonly IApiCaller _apiCaller;

        public MerchantAppService(IApiCaller apiCaller)
        {
            _apiCaller = apiCaller;
        }

        public async Task<BaseResponse<MerchantViewModel>> GetById(long id)
        {
            var queryParams = new Dictionary<string, string>()
            {
                {"idn", id.ToString()}
            };
            return await _apiCaller.GetAsJson<MerchantViewModel>(
                MicroServicesURL.BaseFundamentalsURL,
                ControllerNames.Fundamentals.Merchant,
                ServiceNameCommon.GetById,
                queryParams
            ).ConfigureAwait(false);
        }


        public async Task<BaseResponse<MerchantViewModel>> GetByMerchantId(long merchantId)
        {
            var queryParams = new Dictionary<string, string>()
            {
                {"merchantId", merchantId.ToString()}
            };
            return await _apiCaller.GetAsJson<MerchantViewModel>(
                MicroServicesURL.BaseFundamentalsURL,
                ControllerNames.Fundamentals.Merchant,
                ServiceNameCommon.GetByMerchantId,
                queryParams
            );
        }

        public async Task<BaseResponse<long>> GetMerchantRefIdByMerchantIdAsync(long merchantId)
        {
            var queryParams = new Dictionary<string, string>()
            {
                {"merchantId", merchantId.ToString()}
            };
            return await _apiCaller.GetAsJson<long>(
                MicroServicesURL.BaseFundamentalsURL,
                ControllerNames.Fundamentals.Merchant,
                ServiceNameLookup.GetMerchantRefIdByMerchantId,
                queryParams
            ).ConfigureAwait(false);
        }

        public async Task<MPCSSAccountPaymentDataModel> GetMPCSSAccountPaymentDataModel(long merchantRefId)
        {
            var queryParams = new Dictionary<string, string>()
            {
                {"merchantRefId", merchantRefId.ToString()}
            };
            var res =  await _apiCaller.GetAsJson<MPCSSAccountPaymentDataModel>(
                MicroServicesURL.BaseFundamentalsURL,
                ControllerNames.Fundamentals.Merchant,
                ServiceNameLookup.GetMPCSSAccountPaymentDataModel,
                queryParams
            ).ConfigureAwait(false);

            if (res.Success == false)
                throw new BusinessException(res.ErrorList, res.ResponseCode);

            return res.Data;
        }

        public async Task<BaseResponse<OmanNetCardAccountViewModel>> GetOmanNetCardAccountByMerchantRefId(long merchantId)
        {
            var queryParams = new Dictionary<string, string>()
            {
                {"merchantId", merchantId.ToString()}
            };
            return await _apiCaller.GetAsJson<OmanNetCardAccountViewModel>(
                MicroServicesURL.BaseFundamentalsURL,
                ControllerNames.Fundamentals.Merchant,
                ServiceNameLookup.GetOmanNetCardAccountByMerchantRefId,
                queryParams
            ).ConfigureAwait(false);
        }
    }
}