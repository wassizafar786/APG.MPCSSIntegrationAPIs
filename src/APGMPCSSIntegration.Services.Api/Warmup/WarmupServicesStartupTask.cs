using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace APGMPCSSIntegration.Services.Api.Warmup
{

    public interface IStartupTask
    {
        Task ExecuteAsync(CancellationToken cancellationToken = default);
    }

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddStartupTask<T>(this IServiceCollection services)
            where T : class, IStartupTask =>
            services.AddTransient<IStartupTask, T>();
    }

    public static class StartupTaskWebHostExtensions
    {
        public static async Task InitAsync(this IHost host)
        {
            if (host == null)
                throw new ArgumentNullException(nameof(host));

            using var scope = host.Services.CreateScope();

            var startupTasks = scope.ServiceProvider.GetServices<IStartupTask>();
            foreach (var startupTask in startupTasks)
                await startupTask.ExecuteAsync(CancellationToken.None);
        }
    }

    public class WarmupServicesStartupTask : IStartupTask
    {
        private readonly IServiceCollection _services;
        private readonly IServiceProvider _provider;

        public WarmupServicesStartupTask(IServiceCollection services, IServiceProvider provider)
        {
            _services = services;
            _provider = provider;
        }

        public Task ExecuteAsync(CancellationToken cancellationToken)
        {
            using (var scope = _provider.CreateScope())
            {
                foreach (var service in GetServices(_services))
                    scope.ServiceProvider.GetServices(service);
            }

            return Task.CompletedTask;
        }

        static IEnumerable<Type> GetServices(IServiceCollection services)
        {
            return services
                .Where(descriptor => descriptor.ImplementationType != typeof(WarmupServicesStartupTask))
                .Where(descriptor => descriptor.ServiceType.ContainsGenericParameters == false)
                .Select(descriptor => descriptor.ServiceType)
                .Distinct();
        }
    }
}
