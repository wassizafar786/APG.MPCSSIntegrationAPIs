using APGDigitalIntegration.IAL.Internal.Interfaces.APGFundamental;
using APGDigitalIntegration.IAL.Internal.Services.APGFundamental;
using APGExecutions.IAL.Internal.Interfaces.APGFundamentals;
using APGMPCSSIntegration.Constant;

namespace APGFundamentals.Application.Helper;

public interface IDateTimeProvider
{
    /// <summary>
    /// Gets DatetimeOffset according to  the specified bank
    /// Value will be cached on request basis
    /// </summary>
    /// <param name="bankId"></param>
    /// <param name="useCache">Always get updated time</param>
    /// <returns></returns>
    Task<DateTimeOffset> NowByBankId(long bankId);
    
    /// <summary>
    /// Gets System DatetimeOffset 
    /// </summary>
    /// <returns></returns>
    Task<DateTimeOffset> SystemNow();

    /// <summary>
    /// Gets DatetimeOffset according to  the specified merchant
    /// Value will be cached on request basis
    /// </summary>
    /// <param name="merchantRefId"></param>
    /// <param name="useCache">Always get updated time</param>
    /// <returns></returns>
    // Task<DateTimeOffset> NowByMerchantRefId(long merchantRefId);
}

public class DatetimeOffsetProvider : IDateTimeProvider
{
    private readonly Dictionary<long, float> _bankOffsetCache; // Per Request Cache {BankId - UTC Offset} // Modify to concurrent dictionary when need
    private readonly Dictionary<long, float> _merchantOffsetCache; // Per Request Cache {MerchantRefId - UTC Offset}  // Modify to concurrent dictionary when need
    private static float? _systemOffset; // Per Service Lifetime Cache // does not change after start.
    
    private readonly IConfParamHelperService _confParamHelperService;
    private readonly IMerchantAppService _merchantAppService;
    private readonly IBankApiService _bankApiService;

    public DatetimeOffsetProvider(IConfParamHelperService confParamHelperService, IMerchantAppService merchantAppService, IBankApiService bankApiService)
    {
        _confParamHelperService = confParamHelperService;
        _merchantAppService = merchantAppService;
        _bankApiService = bankApiService;

        _bankOffsetCache = new Dictionary<long, float>();
        _merchantOffsetCache = new Dictionary<long, float>();
    }
    
    public async Task<DateTimeOffset> NowByBankId(long bankId)
    {
        float offset;
        if (_bankOffsetCache.ContainsKey(bankId) == false)
        {
            offset = await GetBankOffset(bankId);
            _bankOffsetCache.Add(bankId, offset);
        }
        else
        {
            offset =  _bankOffsetCache[bankId];
        }
        
        var now = DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(offset));
        
        return now;
    }
    
    // public  async Task<DateTimeOffset> NowByMerchantRefId(long merchantRefId)
    // {
    //     float offset;
    //     if (_merchantOffsetCache.ContainsKey(merchantRefId) == false)
    //     {
    //         var bankId = await _merchantRepository.GetBankIdByMerchantRefIdAsync(merchantRefId);
    //         offset = await GetBankOffset(bankId);
    //         _merchantOffsetCache.Add(merchantRefId, offset);
    //     }
    //     else
    //     {
    //         offset =  _merchantOffsetCache[merchantRefId];
    //     }
    //     var now = DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(offset));
    //     return now;
    // }

    private async Task<float> GetBankOffset(long bankId)
    {
        var bank = await _bankApiService.GetBank(bankId);
        var offset = bank.Data.BankConfiguration.TimeZoneOffset;
        return offset;
    }

    public async Task<DateTimeOffset> SystemNow()
    {
        _systemOffset ??= await _confParamHelperService.GetValue<float>(ConfigParam.SystemTimezoneOffset);

        return DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(_systemOffset.Value));
    }
}