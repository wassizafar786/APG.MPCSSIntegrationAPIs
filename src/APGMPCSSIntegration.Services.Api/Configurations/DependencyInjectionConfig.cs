using System;
using APGDigitalIntegration.Infra.CrossCutting.IoC;
using Microsoft.Extensions.DependencyInjection;

namespace APGDigitalIntegration.Services.Api.Configurations
{
    public static class DependencyInjectionConfig
    {
        public static void AddDependencyInjectionConfiguration(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            NativeInjectorBootStrapper.RegisterServices(services);
        }
    }
}