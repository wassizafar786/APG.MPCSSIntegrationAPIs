using APG.MessageQueue.Mpcss;
using APGDigitalIntegration.Application.Interfaces;
using APGDigitalIntegration.Cache.Interfaces;
using APGDigitalIntegration.Constant;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace APGDigitalIntegration.Application.Services
{
    public class HealthCheckAppService : IHealthCheckAppService
    {
        private readonly HeartBeatStatus _heartBeatStatus;
        private readonly IServiceProvider _serviceProvider;
        private readonly HealthCheckService _mchealthCheckService;

        public HealthCheckAppService(HeartBeatStatus heartBeatStatus, IServiceProvider serviceProvider, HealthCheckService mchealthCheckService)
        {
            _heartBeatStatus = heartBeatStatus;
            _serviceProvider=serviceProvider;
            _mchealthCheckService=mchealthCheckService;
        }
        public DateTime? GetLastHeartBeatReceived()
        {
            return _heartBeatStatus.LastHeartBeatReceived;
        }
        
        public string GetActiveMqHealth()
        {
            bool isDown =!_heartBeatStatus.LastHeartBeatReceived.HasValue ||
                    _heartBeatStatus.LastHeartBeatReceived.GetValueOrDefault().AddMinutes(15) < DateTime.UtcNow;


          return !isDown ? CommonConstant.PrometheusHealthCheckSuccessMessage : CommonConstant.PrometheusHealthCheckFailedMessage;
        }
        public async Task<string> GetDbConnectionHealth()
        {
            return await GetHealthStatusByKey(CommonConstant.db_health_check_name) == HealthStatus.Healthy ? CommonConstant.PrometheusHealthCheckSuccessMessage : CommonConstant.PrometheusHealthCheckFailedMessage;
        }

        public async Task<string> GetMassTransitHealth()
        {
            return await GetHealthStatusByKey(CommonConstant.mass_transit_health_check_name) == HealthStatus.Healthy ? CommonConstant.PrometheusHealthCheckSuccessMessage : CommonConstant.PrometheusHealthCheckFailedMessage;
        }

        public string GetRedisHealth()
        {
            return _serviceProvider.GetService<ICacheService>().IsConnected() ? CommonConstant.PrometheusHealthCheckSuccessMessage : CommonConstant.PrometheusHealthCheckFailedMessage;
        }

        private async Task<HealthStatus> GetHealthStatusByKey(string healthCheckKey)
        {
            var allChecks = await _mchealthCheckService.CheckHealthAsync();

            var status = allChecks.Entries
                .Where(check => check.Key == healthCheckKey)
                .FirstOrDefault().Value.Status;

            return status;
        }
    }
}
