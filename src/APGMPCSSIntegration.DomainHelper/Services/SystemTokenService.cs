using APGDigitalIntegration.DomainHelper.Interfaces;
using APGDigitalIntegration.DomainHelper.Models;
using APGDigitalIntegration.DomainHelper.ViewModels;
using APGMPCSSIntegration.Constant;
using Jose;
using Microsoft.Extensions.Options;

namespace APGDigitalIntegration.DomainHelper.Services
{
    public class SystemTokenService : ISystemTokenService
    {
        private readonly byte[] _secretKey;
        private readonly int _expiryInMinutes;

        public SystemTokenService(IOptions<SystemAuthenticationConfig> systemAuthConfig)
        {
            _secretKey = Convert.FromBase64String(systemAuthConfig.Value.SecretKeyBase64);
            _expiryInMinutes = systemAuthConfig.Value.SystemTokenExpireyInMinutes;
        }
        public string GenerateSystemToken()
        {
            byte[] secretKey = _secretKey;

            int expiryInMinutes = _expiryInMinutes;

            var now = DateTime.UtcNow;

            var payload = new SystemTokenPayload
            {
                Id = Guid.NewGuid().ToString(),
                CreationDate = now.ToString(),
                ExpireyDate = now.AddMinutes(expiryInMinutes).ToString(),
                RequestSource = RequestSources.APGExecution.ToString(),
            };

            return JWT.Encode(payload, secretKey, JweAlgorithm.DIR, JweEncryption.A128CBC_HS256);
        }

    }
}