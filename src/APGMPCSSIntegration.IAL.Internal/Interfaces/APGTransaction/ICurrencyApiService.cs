namespace APGDigitalIntegration.IAL.Internal.Interfaces.APGTransaction;

public interface ICurrencyApiService
{
    public Task<string> GetCurrencyShortNameByCurrencyId(int currencyId);
    public Task<int> GetCurrencyIdByCurrencyShortName(string currencyShortName);
}

public class CurrencyApiService : ICurrencyApiService
{
    public async Task<string> GetCurrencyShortNameByCurrencyId(int currencyId)
    {
        return currencyId switch
        {
            512 => "OMR",
            _ => throw new InvalidOperationException("Unhandled Currency")
        };
    }

    public async Task<int> GetCurrencyIdByCurrencyShortName(string currencyShortName)
    {
        return currencyShortName switch
        {
            "OMR" => 512,
            _ => throw new InvalidOperationException("Unhandled Currency")
        };    
    }
}