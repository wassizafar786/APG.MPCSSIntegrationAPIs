using StackExchange.Redis;
using static APGDigitalIntegration.Cache.Services.RedisCacheService;

namespace APGDigitalIntegration.Cache.Interfaces;

public interface ICacheService
{
    #region Get Redis Connection Status
    bool IsConnected();
    #endregion

    #region Get Or Add Async
    Task<TData> GetOrAddAsync<TData, TFactoryParam>(string key, Func<TFactoryParam, Task<TData>> factory, TFactoryParam factoryParameter);

    #endregion

    #region Add Key With Value

    /// <summary>
    /// AddKeyWithValue
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <typeparam name="TValue"></typeparam>
    void AddKeyWithValue<TValue>(string key, TValue value);

    #endregion

    #region Add Key With Value Async

    Task AddKeyWithValueAsync<TValue>(string key, TValue value);

    #endregion

    #region Add List As Bulk

    /// <summary>
    /// AddListAsBulk
    /// </summary>
    /// <param name="cachedItems"></param>
    /// <param name="keySelector"></param>
    /// <typeparam name="TItem"></typeparam>
    void AddListAsBulk<TItem>(List<TItem> cachedItems, Func<TItem, string> keySelector);

    #endregion

    #region Add List As Bulk Async

    Task AddListAsBulkAsync<TItem>(List<TItem> cachedItems, Func<TItem, string> keySelector);

    #endregion

    #region Remove All Keys That Starts With

    /// <summary>
    /// Remove Any Keys that start with: keyPattern
    /// <br/>
    /// </summary>
    /// <param name="keyPattern"></param>
    void RemoveAllKeysThatStartsWith(string keyPattern);

    #endregion

    #region Get Value By Key

    TData GetValueByKey<TData>(string key);

    #endregion

    #region Get Value By Key Async

    Task<TData> GetValueByKeyAsync<TData>(string key);

    #endregion

    #region Remove Single Key From Cache

    void RemoveSingleKeyFromCache(string key);

    #endregion

    #region Remove Single Key From Cache Async

    Task RemoveSingleKeyFromCacheAsync(string key);

    #endregion

    Task GetSubScribe(string messageId, SubscriberDelegate keySelector);
    Task Publish(string messageId, string publishMessage);
}