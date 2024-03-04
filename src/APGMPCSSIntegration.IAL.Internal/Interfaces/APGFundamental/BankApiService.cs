using APGDigitalIntegration.Cache.Interfaces;
using APGDigitalIntegration.IAL.Internal.Communicator;
using APGDigitalIntegration.IAL.Internal.Services.APGFundamental;
using APGMPCSSIntegration.Constant;
using APGMPCSSIntegration.IAL.Internal.CacheHelper;
using APGMPCSSIntegration.IAL.Internal.Communicator;
using APGMPCSSIntegration.IAL.Internal.Viewmodel.APGFundamentals.Bank;

namespace APGDigitalIntegration.IAL.Internal.Interfaces.APGFundamental;

public class BankApiService : IBankApiService
{
    private readonly IApiCaller _apiCaller;
    private readonly ICacheService _cacheService;

    public BankApiService(IApiCaller apiCaller, ICacheService cacheService)
    {
        _apiCaller = apiCaller;
        _cacheService = cacheService;
    }

    public async Task<BaseResponse<BankViewModel>> GetBank(long idn)
    {
        var cacheKey = CacheKeysHelper.GetKey(new BankCacheKeyModel(idn));
        var banks = _cacheService.GetValueByKey<BankViewModel>(cacheKey);

        if (banks is not null)
            return BaseResponse<BankViewModel>.GetSuccessResponse(banks);

        var formUrlParameters = new Dictionary<string, string>()
        {
            {nameof(idn), idn.ToString()}
        };

        return await _apiCaller.GetAsJson<BankViewModel>(
            MicroServicesURL.BaseFundamentalsURL,
            ControllerNames.Fundamentals.Bank,
            ServiceNameCommon.GetById,
            formUrlParameters);
    }
        
}