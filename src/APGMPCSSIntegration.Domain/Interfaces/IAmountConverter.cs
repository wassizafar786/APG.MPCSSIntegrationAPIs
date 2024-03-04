using System;

namespace APGDigitalIntegration.Domain.Interfaces;

public interface IAmountConverter
{
    public decimal ConvertToLower(decimal amount, int currencyId);
    public decimal ConvertToHigher(decimal amount, int currencyId);
}

public class AmountConverter : IAmountConverter
{
    public decimal ConvertToLower(decimal amount, int currencyId)
    {
        return currencyId switch
        {
            512 => amount * 1000,
            _ => throw new NotImplementedException()
        };
    }

    public decimal ConvertToHigher(decimal amount, int currencyId)
    {
        return currencyId switch
        {
            512 => amount / 1000,
            _ => throw new NotImplementedException()
        };
    }
}