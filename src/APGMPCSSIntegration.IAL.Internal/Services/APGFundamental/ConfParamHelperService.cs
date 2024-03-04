using APGDigitalIntegration.Cache.Interfaces;
using APGDigitalIntegration.IAL.Internal.Communicator;
using APGExecutions.IAL.Internal.Interfaces.APGFundamentals;
using APGMPCSSIntegration.Constant;
using APGMPCSSIntegration.Constant.Helpers.Services;
using APGMPCSSIntegration.DomainHelper.Exceptions;
using APGMPCSSIntegration.IAL.Internal.CacheHelper;
using APGMPCSSIntegration.IAL.Internal.Communicator;
using APGMPCSSIntegration.IAL.Internal.Viewmodel.APGFundamentals;
using Microsoft.Extensions.Localization;

namespace APGMPCSSIntegration.IAL.Internal.Services.APGFundamentals
{
    public class ConfParamHelperService : IConfParamHelperService
    {
        private readonly IApiCaller _apiCaller;
        private readonly ICacheService _cacheService;
        private readonly IStringLocalizer<ConfParamHelperService> _localizer;


        public ConfParamHelperService(IApiCaller apiCaller, ICacheService cacheService, IStringLocalizer<ConfParamHelperService> localizer)
        {
            _cacheService = cacheService;
            _localizer = localizer;
            _apiCaller = apiCaller;
        }
        
        public async Task<T> GetValue<T>(ConfigParam configParam, long? bankId = null)
        {
            var confParamResponse = await GetConfParam(configParam, bankId).ConfigureAwait(false);
            if (confParamResponse.Success == false)
                throw new BusinessException($"Unable to get Conf Parameter value {configParam}","45321");

            if (confParamResponse.Data is null || confParamResponse.Data.IsDeleted)
                return default;
                
            var confParamValue = confParamResponse.Data.ParamValue;
            if(confParamResponse.Data.IsEncrypted)
                confParamValue = RC5DataEncryption.DecryptData(confParamValue);
            
            return typeof(T).IsEnum
                ? (T)Enum.Parse(typeof(T), confParamValue)
                : (T)Convert.ChangeType(confParamValue, typeof(T));
        }
        public async Task<BaseResponse<ConfParameterViewModel>> GetConfParam(ConfigParam paramName, long? bankId)
        {
            var cacheKey = CacheKeysHelper.GetKey(new ConfParameterCacheKeyModel(bankId, paramName.ToString()));
            var confParamViewModel = await _cacheService.GetValueByKeyAsync<ConfParameterViewModel>(cacheKey).ConfigureAwait(false);

            if (confParamViewModel is not null)
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