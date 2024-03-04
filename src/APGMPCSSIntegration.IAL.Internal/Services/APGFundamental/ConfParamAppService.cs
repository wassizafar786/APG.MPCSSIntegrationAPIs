using APGDigitalIntegration.Cache.Interfaces;
using APGDigitalIntegration.IAL.Internal.Communicator;
using APGMPCSSIntegration.Constant;
using APGMPCSSIntegration.IAL.Internal.CacheHelper;
using APGMPCSSIntegration.IAL.Internal.Communicator;
using APGMPCSSIntegration.IAL.Internal.Interfaces.APGFundamentals;
using APGMPCSSIntegration.IAL.Internal.Viewmodel.APGFundamentals;
using Microsoft.Extensions.Localization;

namespace APGMPCSSIntegration.IAL.Internal.Services.APGFundamentals
{
    public class ConfParamAppService : IConfParamAppService
    {
        private readonly IApiCaller _apiCaller;
        private readonly ICacheService _cacheService;
        private readonly IStringLocalizer<ConfParamAppService> _localizer;

        public ConfParamAppService(IApiCaller apiCaller, ICacheService cacheService, IStringLocalizer<ConfParamAppService> localizer)
        {
            _apiCaller = apiCaller;
            _cacheService = cacheService;
            _localizer = localizer;
        }
      
        
        public async Task<BaseResponse<ConfParameterViewModel>> GetConfParam(ConfigParam paramName, long? bankId)
        {
            var cacheKey = CacheKeysHelper.GetKey(new ConfParameterCacheKeyModel(bankId, paramName.ToString()));
            var confParamViewModel = await _cacheService.GetValueByKeyAsync<ConfParameterViewModel>(cacheKey).ConfigureAwait(false);
            
            if(confParamViewModel is not null)
                return await Task.FromResult(BaseResponse<ConfParameterViewModel>.GetSuccessResponse(confParamViewModel, _localizer[ResponseMessages.Success])).ConfigureAwait(false);

            #region Get From Fundamentals

            var formUrlParameters = new Dictionary<string, string>()
            {
                {nameof(paramName), paramName.ToString()},
                {nameof(bankId), bankId?.ToString()}
            };

            var confParam = await _apiCaller.GetAsJson<ConfParameterViewModel>(
                MicroServicesURL.BaseFundamentalsURL, 
                ControllerNames.Fundamentals.ConfParameter, 
                ServiceName.Fundamentals.GetByParamKey,
                formUrlParameters).ConfigureAwait(false);

            return confParam;

            #endregion
        }
    }
}