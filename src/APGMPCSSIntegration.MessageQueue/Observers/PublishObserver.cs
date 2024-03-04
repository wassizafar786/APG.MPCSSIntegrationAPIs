using APGDigitalIntegration.Constant;
using APGDigitalIntegration.DomainHelper.Interfaces;
using APGDigitalIntegration.DomainHelper.ViewModels;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace APG.MessageQueue.Observers
{

    public class PublishObserver : IPublishObserver
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ISystemTokenService _systemTokenService;
        private readonly bool _systemAuthEnabled;


        public PublishObserver(IHttpContextAccessor httpContextAccessor, ISystemTokenService systemTokenService, IOptions<SystemAuthenticationConfig> systemAuthConfig)
        {
            _httpContextAccessor = httpContextAccessor;
            _systemTokenService = systemTokenService;
            _systemAuthEnabled = systemAuthConfig.Value.Enabled;
        }

        public Task PostPublish<T>(PublishContext<T> context) where T : class
        {
            return Task.CompletedTask;
        }

        public Task PrePublish<T>(PublishContext<T> context) where T : class
        {
            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Production" || _systemAuthEnabled)
            {
                string systemToken = _systemTokenService.GenerateSystemToken();
                context.Headers.Set(CommonConstant.SystemToken, systemToken);
            }

            string correlationId = _httpContextAccessor?.HttpContext?.Request?.Headers?[CommonConstant.CorrelationId];

            if (correlationId != null && Guid.TryParse(correlationId, out Guid correlationIdGuid))
                context.ConversationId = correlationIdGuid;

            return Task.CompletedTask;

        }

        public Task PublishFault<T>(PublishContext<T> context, Exception exception) where T : class
        {
            return Task.CompletedTask;
        }
    }
}