using APGDigitalIntegration.Constant;
using APGDigitalIntegration.DomainHelper.Models;
using APGDigitalIntegration.DomainHelper.ViewModels;
using MassTransit;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
namespace APG.MessageQueue.Observers
{
    public class ConsumeObserver : IConsumeObserver
    {
        private readonly byte[] _secretKey;
        private readonly bool _systemAuthEnabled;
        public ConsumeObserver(IOptions<SystemAuthenticationConfig> systemAuthConfig)
        {
            _secretKey = Convert.FromBase64String(systemAuthConfig.Value.SecretKeyBase64) ?? throw new ArgumentNullException("System Token Secret Key Not Provided");

            _systemAuthEnabled = systemAuthConfig.Value.Enabled;
        }

        public Task ConsumeFault<T>(ConsumeContext<T> context, Exception exception) where T : class
        {
            return Task.CompletedTask;
        }

        public Task PostConsume<T>(ConsumeContext<T> context) where T : class
        {
            return Task.CompletedTask;
        }

        public Task PreConsume<T>(ConsumeContext<T> context) where T : class
        {

            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Production" || _systemAuthEnabled)
            {

                var token = context.Headers.Get<string>(CommonConstant.SystemToken);

                if (string.IsNullOrEmpty(token))
                    throw new MassTransitException("Unauthorized: Missing SystemToken header");

                try
                {
                    string decrypedPayload = Jose.JWT.Decode(token, _secretKey);

                    SystemTokenPayload model = JsonConvert.DeserializeObject<SystemTokenPayload>(decrypedPayload);

                    if (!model.IsExpiryDateValid())
                        throw new MassTransitException("Unauthorized: Expired Token");

                    return Task.CompletedTask;

                }
                catch (Exception ex)
                {
                    throw new MassTransitException("Unauthorized", ex);
                }
            }

            return Task.CompletedTask;
        }
    }
}