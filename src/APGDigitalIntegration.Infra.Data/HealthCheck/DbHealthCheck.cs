using APGDigitalIntegration.Infra.Data.Context;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace APGDigitalIntegration.Infra.Data.HealthCheck
{
    public class DbHealthCheck : IHealthCheck
    {
        private readonly IConfiguration _configuration;
        protected readonly APGDigitalIntegrationContext Db;

        public DbHealthCheck(IConfiguration configuration, APGDigitalIntegrationContext db)
        {
            _configuration=configuration;
            Db=db;
        }


        public async Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var ctWithTimeOut = new CancellationTokenSource(TimeSpan.FromSeconds(7)).Token;
            try
            {
                var result =await Db.Ping(ctWithTimeOut);
                if(result)
                    return HealthCheckResult.Healthy("connection success");
                return HealthCheckResult.Unhealthy("connection failed");
            }
            catch (Exception)
            {
                return HealthCheckResult.Unhealthy("connection failed");
            }
        }
    }
}
