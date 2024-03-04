using APGDigitalIntegration.Application.Interfaces;
using APGDigitalIntegration.Application.ViewModels;
using APGMPCSSIntegration.Application.Services.Messages;
using APGMPCSSIntegration.Constant;
using APGMPCSSIntegration.Services.Api.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Threading.Tasks;

namespace APGDigitalIntegration.Services.Api.Controllers
{

    [AllowAnonymous]
    [Route(ServiceName.Controllers.HealthCheck)]

    public class HealthCheckController : ApiController
    {
        private readonly IHealthCheckAppService _healthCheckAppService;

        public HealthCheckController(IHealthCheckAppService healthCheckAppService, CurrentUserDataViewModel currentUserDataViewModel, IHttpContextAccessor httpContextAccessor):base(currentUserDataViewModel,httpContextAccessor)
        {
            _healthCheckAppService = healthCheckAppService;
        }

        [HttpGet]
        [Route(HealthCheckServicesName.ActiveMq)]
        public IActionResult GetLastHeartbeatReceived()
        {
            return Content(_healthCheckAppService.GetActiveMqHealth(), "text/plain");
        }   
        
        [HttpGet]
        [Route(HealthCheckServicesName.Redis)]
        public IActionResult RedisHealthCheck()
        {
            return Content(_healthCheckAppService.GetRedisHealth(), "text/plain");
        }


        [HttpGet]
        [Route(HealthCheckServicesName.Db)]
        public async Task<IActionResult> DbConnection()
        {
            return Content(await _healthCheckAppService.GetDbConnectionHealth(), "text/plain");
        }

        [HttpGet]
        [Route(HealthCheckServicesName.MassTransit)]
        public async Task<IActionResult> MassTransitHealthCheck()
        {
            return Content(await _healthCheckAppService.GetMassTransitHealth(), "text/plain");
        }
    }
}
