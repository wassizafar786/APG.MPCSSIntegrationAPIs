using APGMPCSSIntegration.IAL.Internal.Communicator;
using APGMPCSSIntegration.IAL.Internal.Viewmodel.APGFundamentals.Bank;

namespace APGDigitalIntegration.IAL.Internal.Services.APGFundamental;

public interface IBankApiService
{
    Task<BaseResponse<BankViewModel>> GetBank(long idn);
}