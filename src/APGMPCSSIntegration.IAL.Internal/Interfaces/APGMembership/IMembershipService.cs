
using APGDigitalIntegration.IAL.Internal.Models.APGMembership;
using APGMPCSSIntegration.IAL.Internal.Communicator;

namespace APGDigitalIntegration.IAL.Internal.Interfaces.APGMembership
{
    public interface IMembershipService
    {
        Task<BaseResponse<CheckPasswordResponseViewModel>> CheckPassword(CheckPasswordRequestViewModel checkPasswordRequest);
    }
}