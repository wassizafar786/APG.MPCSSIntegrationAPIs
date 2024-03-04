using APGDigitalIntegration.Cache.Interfaces;
using APGDigitalIntegration.IAL.Internal.ViewModels.APGTransaction;
using APGMPCSSIntegration.IAL.Internal.CacheHelper;

namespace APGDigitalIntegration.BackgroundJobs.Helpers;

public class TransactionTypesCacheService
{
    private readonly ICacheService _cacheService;
    private readonly Dictionary<int, TransactionTypeCacheModel> TransactionTypes;
    

    public TransactionTypesCacheService(ICacheService cacheService)
    {
        TransactionTypes = new Dictionary<int, TransactionTypeCacheModel>();

        _cacheService = cacheService;
    }

    public async Task<TransactionTypeCacheModel> Get(int transactionTypeId)
    {
        if (TransactionTypes.ContainsKey(transactionTypeId))
            return TransactionTypes[transactionTypeId];

        var cacheKey = CacheKeysHelper.GetKey(new TransactionTypeCacheKeyModel(transactionTypeId));
        var transactionTypeCacheModel = await _cacheService.GetValueByKeyAsync<TransactionTypeCacheModel>(cacheKey);
        TransactionTypes.Add(transactionTypeId, transactionTypeCacheModel);

        return transactionTypeCacheModel;
    }
}