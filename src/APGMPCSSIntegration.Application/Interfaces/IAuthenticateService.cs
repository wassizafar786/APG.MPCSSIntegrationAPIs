
using APGDigitalIntegration.IAL.Internal.Models.APGMembership;
using System.Threading.Tasks;

namespace APGDigitalIntegration.Application.Interfaces
{
    public interface IAuthenticateService
    {
        Task<CheckPasswordResponseViewModel> CheckPassword(string password, int requestSourceId);

    }
}
