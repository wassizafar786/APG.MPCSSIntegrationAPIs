using System;
using System.Threading;
using System.Threading.Tasks;
using APGDigitalIntegration.Cache.Interfaces;
using APGExecutions.IAL.Internal.Interfaces.APGFundamentals;
using APGMPCSSIntegration.Constant;
using APGMPCSSIntegration.IAL.Internal.CacheHelper;
using APGMPCSSIntegration.IAL.Internal.Viewmodel.APGFundamentals;
using Microsoft.Extensions.Configuration;

namespace APGMPCSSIntegration.Services.Api.Warmup;

public class MicroServiceURLStartupTask : IStartupTask
{
    private readonly IConfParamHelperService _confParamHelperService;
    private readonly IConfiguration _configuration;
    private readonly ICacheService _cacheService;

    public MicroServiceURLStartupTask(IConfParamHelperService confParamHelperService, IConfiguration configuration, ICacheService cacheService)
    {
        _confParamHelperService = confParamHelperService;
        _configuration = configuration;
        _cacheService = cacheService;
    }

    public async Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        var fundamentalsURL = await GetFundamentalsMicroServiceUrl();
        if (string.IsNullOrWhiteSpace(fundamentalsURL))
            throw new InvalidOperationException(TechnicalErrorMessages.FundamentalsMicroServiceURLIsNotConfigured);
        
        MicroServicesURL.BaseFundamentalsURL.Value = fundamentalsURL;

        var membershipURL = await _confParamHelperService.GetValue<string>(ConfigParam.BaseMembershipMicroServiceUrl);
        if (string.IsNullOrWhiteSpace(membershipURL))
            throw new InvalidOperationException(TechnicalErrorMessages.MembershipMicroServiceURLIsNotConfigured);
        
        MicroServicesURL.BaseMembershipURL.Value = membershipURL;
        
        var transactionUrl = await _confParamHelperService.GetValue<string>(ConfigParam.BaseTransactionsMicroServiceUrl);
        if (string.IsNullOrWhiteSpace(transactionUrl))
            throw new InvalidOperationException(TechnicalErrorMessages.TransactionsMicroServiceURLIsNotConfigured);
        
        MicroServicesURL.BaseTransactionsURL.Value = transactionUrl;
        
        var apgLogUrl = await _confParamHelperService.GetValue<string>(ConfigParam.BaseLogsMicroServiceUrl);
        if (string.IsNullOrWhiteSpace(apgLogUrl))
            throw new InvalidOperationException(TechnicalErrorMessages.LogsMicroServiceURLIsNotConfigured);
        
        MicroServicesURL.BaseAPGLogURL.Value = apgLogUrl;
    }
    
    private async Task<string> GetFundamentalsMicroServiceUrl()
    {
        var fundamentalsCacheKey = CacheKeysHelper.GetKey(new ConfParameterCacheKeyModel(null, ConfigParam.BaseFundamentalsMicroServiceUrl.ToString()));

        var fundamentalsViewModel = await _cacheService.GetValueByKeyAsync<ConfParameterViewModel>(fundamentalsCacheKey);
        if (fundamentalsViewModel is not null && !string.IsNullOrWhiteSpace(fundamentalsViewModel.ParamValue) && !fundamentalsViewModel.IsDeleted)
            return fundamentalsViewModel.ParamValue;
        
        return _configuration["APGFundamentalsBackupURL"];
    }
}