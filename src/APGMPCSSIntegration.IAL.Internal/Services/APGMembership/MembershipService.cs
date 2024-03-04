
using APGDigitalIntegration.IAL.Internal.Communicator;
using APGDigitalIntegration.IAL.Internal.Interfaces.APGMembership;
using APGDigitalIntegration.IAL.Internal.Models.APGMembership;
using APGMPCSSIntegration.Constant;
using APGMPCSSIntegration.IAL.Internal.Communicator;

namespace APGMPCSSIntegration.IAL.Internal.Services.APGMembership
{
    public class MembershipService : IMembershipService
    {
        private readonly IApiCaller _apiCaller;

        public MembershipService(IApiCaller apiCaller)
        {
            _apiCaller = apiCaller;
        }

        public async Task<BaseResponse<CheckPasswordResponseViewModel>> CheckPassword(CheckPasswordRequestViewModel checkPasswordRequest)
        {
            return await _apiCaller.PostAsJson<CheckPasswordResponseViewModel>(MicroServicesURL.BaseMembershipURL,
                ServiceName.Membership.ServiceNameMembership,
                ServiceNameCommon.CheckPassword,
                checkPasswordRequest).ConfigureAwait(false);
        }

    }
}