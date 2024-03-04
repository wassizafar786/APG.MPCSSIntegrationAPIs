using System;
using APG.MessageQueue.Mpcss.Options;
using APG.MessageQueue.Options;
using APGMPCSSIntegration.Cache.Options.v1;
using APGDigitalIntegration.Infra.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using APGDigitalIntegration.Constant;

namespace APGDigitalIntegration.Services.Api.Configurations
{
    public static class DatabaseConfig
    {
        public static void AddOptionsConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));


            #region Db
            var connectionString = Environment.GetEnvironmentVariable(CommonConstant.DEFAULT_CONNECTION_VAR) ?? configuration.GetConnectionString(CommonConstant.DefaultConnection);

            services.AddDbContext<APGDigitalIntegrationContext>(options =>
                options.UseNpgsql(connectionString));

            #endregion


            #region ActiveMq
            var activeMqSettings = configuration.GetSection(nameof(ActiveMqConfiguration)).Get<ActiveMqConfiguration>();

            services.Configure<ActiveMqConfiguration>(o =>
            {
                o.ConnectionRecoveryIntervalInSeconds = activeMqSettings.ConnectionRecoveryIntervalInSeconds;
                o.PublishRetryPolicy=activeMqSettings.PublishRetryPolicy;
                o.ActiveMqUrl=activeMqSettings.ActiveMqUrl;
                o.UserName=activeMqSettings.UserName;
                o.Password= Environment.GetEnvironmentVariable(CommonConstant.ACTIVEMQ_PASSWORD_VAR) ?? activeMqSettings.Password.ToString();
                o.HeartBeat=activeMqSettings.HeartBeat;
            });
            #endregion

            #region Redis
            var redisSettings = configuration.GetSection(nameof(RedisConfiguration)).Get<RedisConfiguration>();

            services.Configure<RedisConfiguration>(o =>
            {
                o.Hostname = redisSettings.Hostname.ToString();
                o.UserName = redisSettings.UserName.ToString();
                o.Port = redisSettings.Port.ToString();
                o.Password = Environment.GetEnvironmentVariable(CommonConstant.REDIS_PASSWORD_VAR) ?? redisSettings.Password.ToString();
                o.Enabled = redisSettings.Enabled;
                o.RedisDb = redisSettings.RedisDb;

            });
            #endregion

            // MPCSS Certificate Options
            services.Configure<MPCSSCertificate>(
                configuration.GetSection(nameof(MPCSSCertificate)));
        }
    }
}