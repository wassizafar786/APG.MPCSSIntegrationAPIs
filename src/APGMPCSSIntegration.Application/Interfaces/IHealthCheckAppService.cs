using APGDigitalIntegration.Cache.Interfaces;
using APGDigitalIntegration.Constant;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace APGDigitalIntegration.Application.Interfaces
{
    public interface IHealthCheckAppService
    {
        DateTime? GetLastHeartBeatReceived();
        string GetActiveMqHealth();
        string GetRedisHealth();
        Task<string> GetDbConnectionHealth();
        Task<string> GetMassTransitHealth();
    }

}
