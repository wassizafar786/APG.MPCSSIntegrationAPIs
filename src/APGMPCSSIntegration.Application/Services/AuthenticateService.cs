using APGDigitalIntegration.Application.Interfaces;
using APGDigitalIntegration.IAL.Internal.Interfaces.APGMembership;
using APGDigitalIntegration.IAL.Internal.Models.APGMembership;
using Microsoft.Extensions.Localization;
using System.Threading.Tasks;

namespace APGDigitalIntegration.Application.Services;

public class AuthenticateService : IAuthenticateService
{
    private readonly IMembershipService _membershipService;

    private readonly IStringLocalizer<AuthenticateService> _localizer;

    public AuthenticateService(IMembershipService membershipService, IStringLocalizer<AuthenticateService> localizer)
    {
        _membershipService=membershipService;
        _localizer=localizer;
    }
    public async Task<CheckPasswordResponseViewModel> CheckPassword(string password, int requestSourceId)
    {
        var requestModel = new CheckPasswordRequestViewModel()
        {
            Password=password,
            RequestSourceId=requestSourceId
        };

        var membershipCheckPasswordResponse = await _membershipService.CheckPassword(requestModel).ConfigureAwait(false);

        return membershipCheckPasswordResponse.Data;

    }
}

