using System;
using APGDigitalIntegration.Services.Api.Configurations;
using APGMPCSSIntegration.Constant;
using APGMPCSSIntegration.DomainHelper.Middlewares;
using APGMPCSSIntegration.Services.Api.Resources;
using APGMPCSSIntegration.Services.Api.Warmup;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Localization;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using APG.MessageQueue.Consumers;
using APG.MessageQueue.Consumers.Filters;
using APG.MessageQueue.Consumers.Mpcss.MPCSSConsumers.Heartbeat;
using APG.MessageQueue.Consumers.Mpcss.MPCSSConsumers.Operation;
using APG.MessageQueue.Consumers.Mpcss.MPCSSConsumers.Payment;
using APG.MessageQueue.Interfaces;
using APG.MessageQueue.Mpcss.Configurator;
using APG.MessageQueue.Options;
using APG.MessageQueue.Services;
using APGDigitalIntegration.BackgroundJobs.Services;
using APGDigitalIntegration.Common.CommonViewModels.Common;
using APGDigitalIntegration.Common.CommonViewModels.Heartbeat;
using APGDigitalIntegration.Services.Api.Middleware;
using Hangfire;
using MassTransit;
using APG.MessageQueue.Mpcss;
using APG.MessageQueue.Mpcss.Interfaces;
using APG.MessageQueue.Mpcss.Services;
using Hangfire.PostgreSql;
using Prometheus;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using APGDigitalIntegration.Infra.Data.HealthCheck;
using APGDigitalIntegration.Constant;
using APG.MessageQueue.Observers;
using APGDigitalIntegration.DomainHelper.ViewModels;
using Microsoft.Extensions.Options;

namespace APGDigitalIntegration.Services.Api
{
    public class Startup
    {
        public Startup(IHostEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", true, true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true);

            if (env.IsDevelopment())
            {
                builder.AddUserSecrets<Startup>();
            }

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            // for Posgres DatetimeOffset Issue
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);


            services.AddHangfire(configuration => configuration
        .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
        .UseSimpleAssemblyNameTypeSerializer()
        .UseRecommendedSerializerSettings()
        .UsePostgreSqlStorage(
                Environment.GetEnvironmentVariable(CommonConstant.DEFAULT_CONNECTION_VAR) ?? Configuration.GetConnectionString(CommonConstant.DefaultConnection), new PostgreSqlStorageOptions
        {
            DistributedLockTimeout = TimeSpan.FromMinutes(5) // Set the lock timeout as needed
        }));

            services.AddHangfireServer();

            
            // WebAPI Config
            services.AddControllers();

            // Middlewares
            services.AddScoped<SystemAuthenticationMiddleware>();
            services.AddScoped<CommunicationLogMiddleware>();
            services.AddScoped<ExceptionMiddleware>();
            services.AddSingleton<LocalizationMiddleware>();

            // Setting DBContexts
            services.AddOptionsConfiguration(Configuration);

            // ASP.NET Identity Settings & JWT
            // services.AddApiIdentityConfiguration(Configuration);

            // Interactive AspNetUser (logged in)
            // NetDevPack.Identity dependency
            // services.AddAspNetUserConfiguration();

            // AutoMapper Settings
            services.AddAutoMapperConfiguration();

            // Swagger Config
            services.AddSwaggerConfiguration();

            // Adding MediatR for Domain Events and Notifications
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(AppDomain.CurrentDomain.GetAssemblies()));

            // .NET Native DI Abstraction
            services.AddDependencyInjectionConfiguration();

            services.AddHealthChecks()
                    .AddCheck<DbHealthCheck>(CommonConstant.db_health_check_name);

            #region Add Localization

            services.AddSingleton<IStringLocalizerFactory, JsonStringLocalizerFactory>();

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<HeartBeatStatus>();
            services.AddSingleton<IActiveMQHostedService, ActiveMQHostedService>();
            string defaultLang = Languages.English;


            services.AddLocalization(o => o.ResourcesPath = null);
            services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCultures = new[] { new CultureInfo(Languages.English), new CultureInfo(Languages.Arabic) };

                options.DefaultRequestCulture = new RequestCulture(defaultLang, defaultLang);
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;
                options.RequestCultureProviders.Insert(0, new CustomRequestCultureProvider(context =>
                {
                    var userLangs = context.Request.Headers["Accept-Language"].ToString();
                    var firstLang = userLangs.Split(',').FirstOrDefault();
                    var defaultLangOrUserLang = string.IsNullOrEmpty(firstLang) ? defaultLang : firstLang;
                    return Task.FromResult(new ProviderCultureResult(defaultLangOrUserLang, defaultLangOrUserLang));
                }));
            });
            #endregion


            // Warmup Services
            // services.AddStartupTask<WarmupServicesStartupTask>().TryAddSingleton(services);
            services.AddStartupTask<MicroServiceURLStartupTask>();
            services.AddStartupTask<MPCSSConstantsStartupTask>();
            ThreadPool.SetMinThreads(1000, 1000);

            var systemAuthConfig = new SystemAuthenticationConfig();

            Configuration.GetSection(nameof(SystemAuthenticationConfig)).Bind(systemAuthConfig);

            services.AddOptions<SystemAuthenticationConfig>().Configure(options =>
            {
                options.Enabled = systemAuthConfig.Enabled;
                options.SystemTokenExpireyInMinutes = systemAuthConfig.SystemTokenExpireyInMinutes;
                options.SecretKeyBase64 = Environment.GetEnvironmentVariable(CommonConstant.SYSTEM_TOKEN_SECRET_KEY_VAR) ?? systemAuthConfig.SecretKeyBase64;
            });

            /* Start */
            //Rabbit MQ
            var rabbitmqConfigs = new RabbitMqConfiguration();
            Configuration.GetSection(nameof(RabbitMqConfiguration)).Bind(rabbitmqConfigs);

            services.AddScoped<IMessageQueue, MassTransitMessageBus>();
            var assembly = typeof(IConsumerMarkerClass).Assembly;
            services.AddMassTransit(configurator =>
            {
                if (configurator == null) 
                    throw new ArgumentNullException(nameof(configurator));
                configurator.SetKebabCaseEndpointNameFormatter();
                configurator.AddConsumeObserver<ConsumeObserver>();
                configurator.AddPublishObserver<PublishObserver>();
                configurator.AddConsumers(assembly);
                configurator.UsingRabbitMq((context, cfg) =>
                {
                    cfg.UseConsumeFilter(typeof(ExceptionLoggerFilter<>), context);
                    cfg.UseMessageRetry(c => c.Exponential(5, TimeSpan.FromMilliseconds(200), TimeSpan.FromSeconds(10), TimeSpan.FromMilliseconds(200)));
                    cfg.ConfigureEndpoints(context);
                });
            });

            services.AddOptions<RabbitMqTransportOptions>().Configure(options =>
            {
                options.Host = rabbitmqConfigs.Hostname;
                options.User = rabbitmqConfigs.UserName;
                options.Pass = Environment.GetEnvironmentVariable(CommonConstant.RABBITMQ_PASSWORD_VAR) ?? rabbitmqConfigs.Password;
                options.Port = (ushort)rabbitmqConfigs.Port;
            });

            
            /* END */
            
            
            // Active MQ Listeners
            services.ConfigureActiveMQ(cfg =>
            {
                cfg.AddMessageListener<CustomerNameVerificationConsumer, CustomerNameVerificationConsumerDefinition, Envelope>();
                cfg.AddMessageListener<PaymentEnquiryResponseConsumer, PaymentEnquiryResponseConsumerDefinition, Envelope>();
                cfg.AddMessageListener<InwardPaymentMessageConsumer, InwardPaymentMessageConsumerDefinition, Envelope>();
                cfg.AddMessageListener<PaymentStatusReportConsumer, PaymentStatusReportConsumerDefinition, Envelope>();
                cfg.AddMessageListener<RegistrationConsumer, RegistrationConsumerDefinition, Envelope>();
                cfg.AddMessageListener<HeartbeatConsumer, HeartbeatConsumerDefinition, HeartBeatMessage>();
            });


        }
    
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env,IOptions<SystemAuthenticationConfig> systemAuthConfig)
        {
            if (env.IsProduction() || systemAuthConfig.Value.Enabled)
            {
                app.UseMiddleware<SystemAuthenticationMiddleware>();
            }
            app.UseMiddleware<LocalizationMiddleware>();
            app.UseMiddleware<CommunicationLogMiddleware>();
            app.UseMiddleware<ExceptionMiddleware>();

            app.UseHttpsRedirection();

            app.UseHangfireDashboard("/dashboard");

            app.UseRouting();

            RunBackGroundJobs();
            
            app.UseCors(c =>
            {
                c.AllowAnyHeader();
                c.AllowAnyMethod();
                c.AllowAnyOrigin();
            });

            // NetDevPack.Identity dependency
            // app.UseAuthConfiguration();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapMetrics();
            });

            app.UseSwaggerSetup();
        }
        
        public void RunBackGroundJobs()
        {
            RecurringJob.AddOrUpdate<TransactionTimeoutJob>(service => service.Execute(), Cron.Minutely);
            RecurringJob.AddOrUpdate<TransactionTimeoutEnquiryJob>(service => service.Execute(), Cron.Minutely);
        }
    }
}
