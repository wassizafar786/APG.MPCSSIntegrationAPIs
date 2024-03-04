using System;

namespace APGMPCSSIntegration.IAL.Internal.CacheHelper
{
    public static class CacheKeysHelper
    { 
        public static string GetKey(ICacheKeyModel entity)
        {
            if(entity is null)
                throw new ArgumentNullException(nameof(entity));
            
            return entity.GetKey();
        }
    }

    public interface ICacheKeyModel
    {
        string GetKey();
    }
    
    public record ConfParameterCacheKeyModel(long? BankId, string ParamKey) : ICacheKeyModel
    {
        /// <summary>
        /// Example Key-> ConfParameter:BankId:ConfParamKey
        /// </summary>
        public string GetKey() => $"{CacheNamesConfig.ConfParametersBaseKey}{CacheConfig.Separator}{BankId ?? 0}{CacheConfig.Separator}{ParamKey}";
    }

    public record BankCacheKeyModel(long? BankId) : ICacheKeyModel
    {
        /// Example Key-> Bank:BankId
        public string GetKey() => $"{CacheNamesConfig.BankBaseKey}{CacheConfig.Separator}{BankId}";
    }

    public record MerchantMdrCacheKeyModel(long MerchantRefId, int TerminalTypeId, int ChannelTypeId) : ICacheKeyModel
    {
        /// Example Key-> MerchantMdr:MerchantRefId:TerminalTypeId:ChannelTypeId
        public string GetKey() =>
            $"{CacheNamesConfig.MerchantMdrBaseKey}{CacheConfig.Separator}{MerchantRefId}{CacheConfig.Separator}{TerminalTypeId}{CacheConfig.Separator}{ChannelTypeId}";
    }

    public record RemoveMerchantMdrPatternCacheKeyModel(long MerchantRefId) : ICacheKeyModel
    {
        ///Example Key-> MerchantMdr:MerchantRefId
        /// This is is only used for to generate pattern to delete all mdrs for selected merchant.
        public string GetKey() => $"{CacheNamesConfig.MerchantMdrBaseKey}{CacheConfig.Separator}{MerchantRefId}";
    }

    public record MerchantTransactionLimitsCacheKeyModel(long MerchantRefId) : ICacheKeyModel
    {
        /// Example Key -> MerchantTransactionLimit:MerchantRefId
        public string GetKey() => $"{CacheNamesConfig.MerchantTransactionLimitBaseKey}{CacheConfig.Separator}{MerchantRefId}";
    }
    
    public record TransactionTypeCacheKeyModel(int TransactionTypeId) : ICacheKeyModel
    {
        /// Example Key -> TransactionType:TransactionTypeId
        public string GetKey() => $"{CacheNamesConfig.TransactionTypeBaseKey}{CacheConfig.Separator}{TransactionTypeId}";
    }
}