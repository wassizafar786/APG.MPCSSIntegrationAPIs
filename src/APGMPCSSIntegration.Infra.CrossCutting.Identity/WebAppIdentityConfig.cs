using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetDevPack.Identity;

namespace APGDigitalIntegration.Infra.CrossCutting.Identity
{
    public static class WebAppIdentityConfig
    {
        public static void AddWebAppIdentityConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            // Default EF Context for Identity (inside of the NetDevPack.Identity)
            services.AddIdentityEntityFrameworkContextConfiguration(options =>
                SqlServerDbContextOptionsExtensions.UseSqlServer(options, configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly("APGFundamentals.Infra.CrossCutting.Identity")));

            // Default Identity configuration from NetDevPack.Identity
            services.AddIdentityConfiguration();
        }
    }
}