using APGDigitalIntegration.Constant;
using APGDigitalIntegration.Infra.Data.Context;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetDevPack.Mediator;

Console.WriteLine("=========================");
Console.WriteLine("Begin Run Migration Tools");

var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";

var configurationBuilder = new ConfigurationBuilder()
        .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
        .AddJsonFile($"appsettings.{environmentName}.json", true);

// Build the configuration
var configuration = configurationBuilder.Build();


// Create a service collection
var services = new ServiceCollection();

// Register services

services.AddScoped<MediatR.IMediator, MediatR.Mediator>();
services.AddScoped<APGDigitalIntegrationContext>();

var connectionString = Environment.GetEnvironmentVariable(CommonConstant.DEFAULT_CONNECTION_VAR) ?? configuration.GetConnectionString(CommonConstant.DefaultConnection);

services.AddDbContext<APGDigitalIntegrationContext>(options =>
  options.UseNpgsql(connectionString));

var serviceProvider = services.BuildServiceProvider();

var dbContextOptions = serviceProvider.GetService<DbContextOptions<APGDigitalIntegrationContext>>();
var mediatorHandler = serviceProvider.GetService<IMediatorHandler>();
var httpContextAccessor = serviceProvider.GetService<IHttpContextAccessor>();


Console.WriteLine("Begin Run Migration");

try
{
    // throw new Exception("Erro in code");
    using (var context = new APGDigitalIntegrationContext(dbContextOptions))
    {
        context.Database.SetCommandTimeout(TimeSpan.FromMinutes(30));
        context.Database.Migrate();
    }
}
catch (Exception ex)
{
    Console.WriteLine("============");
    Console.WriteLine("Begin Error");
    Console.WriteLine(ex.Message);
    Console.WriteLine("End Error");
    Console.WriteLine("============");
    throw;
}

Console.WriteLine("End Migration Tools");
Console.WriteLine("=========================");

Console.ReadLine();