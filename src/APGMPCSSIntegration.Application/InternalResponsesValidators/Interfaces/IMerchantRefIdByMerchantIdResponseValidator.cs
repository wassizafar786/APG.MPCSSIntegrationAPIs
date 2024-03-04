
using APGDigitalIntegration.Application.InternalResponsesValidators.Validators;

namespace APGDigitalIntegration.Application.InternalResponsesValidators.Interfaces
{
    public interface IMerchantRefIdByMerchantIdResponseValidator
    {
        MerchantRefIdByMerchantIdResponseValidator AddValidTerminalRefIdValidator();
    }
}