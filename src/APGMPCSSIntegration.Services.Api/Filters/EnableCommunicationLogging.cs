using System;
using System.Linq;
using System.Threading.Tasks;
using APGDigitalIntegration.Application.Interfaces;
using APGDigitalIntegration.Common.CommonServices;
using APGMPCSSIntegration.Constant;
using APGMPCSSIntegration.IAL.Internal.BaseRequests;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace APGDigitalIntegration.Services.Api.Filters;

public class EnableCommunicationLogging : TypeFilterAttribute
{
    public EnableCommunicationLogging(LogSource logSource) 
        : base(typeof(EnableCommunicationLoggingFilter))
    {
        Arguments = new object[] { logSource };
    }
        
    private class EnableCommunicationLoggingFilter : IAsyncActionFilter
    {

        private readonly IMPCSSCommunicationLogService _communicationLogService;
        private readonly LogSource _logSource;

        public EnableCommunicationLoggingFilter(IMPCSSCommunicationLogService communicationLogService, LogSource logSource)
        {
            _logSource = logSource;
            _communicationLogService = communicationLogService;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (_logSource == LogSource.All)
                EnableCommunicationLogging();
            else
            {
                if(context.ActionArguments.Values.FirstOrDefault() is not ISecureHashBase request)
                    throw new ArgumentException(TechnicalErrorMessages.TheRequestMustImplementISecureHashBase);

                if ( ShouldLogMerchantAPIOnly() && IsRequestSourceNotMerchantApi(request))
                    DisableCommunicationLogging();

                else if ( ShouldIgnorePortalOnly() && IsRequestSourcePortal(request))
                    DisableCommunicationLogging();
                
                else
                    EnableCommunicationLogging();
            }
                
            await next().ConfigureAwait(false);
        }

        private void DisableCommunicationLogging() => _communicationLogService.CommunicationLogEnabled = false;
        private void EnableCommunicationLogging() => _communicationLogService.CommunicationLogEnabled = true;
            
        private static bool IsRequestSourcePortal(ISecureHashBase request) => request.RequestSource == (int) RequestSources.Portal;
        private static bool IsRequestSourceNotMerchantApi(ISecureHashBase request) => request.RequestSource != (int) RequestSources.Webhook;
            
        private bool ShouldIgnorePortalOnly() => _logSource == LogSource.IgnorePortalOnly;
        private bool ShouldLogMerchantAPIOnly() => _logSource == LogSource.MerchantApiOnly;

    }
}