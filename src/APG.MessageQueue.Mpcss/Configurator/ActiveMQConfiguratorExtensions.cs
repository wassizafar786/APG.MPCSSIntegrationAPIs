using APG.MessageQueue.Mpcss.ActiveMQTransport;
using APG.MessageQueue.Mpcss.Services;
using Microsoft.Extensions.DependencyInjection;

namespace APG.MessageQueue.Mpcss.Configurator;

public static class ActiveMQConfiguratorExtensions
{
    public static void ConfigureActiveMQ(this IServiceCollection serviceCollection, Action<ActiveMQConfigurator> configurator)
    {
        var configs = new ActiveMQConfigurator(serviceCollection);
        configurator(configs);

        serviceCollection.AddSingleton(_ => configs);
        // Active Mq Messaging 
        serviceCollection.AddSingleton<IActiveMqMessageQueue, NewActiveMQMessageQueue>();
        serviceCollection.AddHostedService<ActiveMQHostedService>();
        serviceCollection.AddHostedService<HeartbeatHostedService>();
        serviceCollection.AddHostedService<ReconnectionActiveMQService>();
        
    } 
}