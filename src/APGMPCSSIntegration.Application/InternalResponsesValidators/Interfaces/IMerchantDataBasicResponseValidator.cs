
using APGDigitalIntegration.Application.InternalResponsesValidators.Validators;

namespace APGDigitalIntegration.Application.InternalResponsesValidators.Interfaces
{
    public interface IMerchantDataBasicResponseValidator
    {
        MerchantDataBasicResponseValidator GetInstance();
        MerchantDataBasicResponseValidator AddCountValidation();
    }
}