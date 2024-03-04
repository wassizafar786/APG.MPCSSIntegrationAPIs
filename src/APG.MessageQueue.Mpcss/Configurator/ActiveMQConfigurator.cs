using System.Collections.ObjectModel;
using APG.MessageQueue.Mpcss.MessageConsuming;
using APG.MessageQueue.Mpcss.MessageHandling;
using Microsoft.Extensions.DependencyInjection;
using Spring.Messaging.Nms.Core;

namespace APG.MessageQueue.Mpcss.Configurator;

public class ActiveMQConfigurator
{
    private readonly IServiceCollection _serviceCollection;
    public ActiveMQConfigurator(IServiceCollection serviceCollection)
    {
        _serviceCollection = serviceCollection;
        DynamicMessageListeners = new Collection<IMessageListener>();
    }

    public Collection<IMessageListener> DynamicMessageListeners { get; set; }

    public ActiveMQConfigurator AddMessageListener<TMessageListener, TMessageListenerDefinition, TMessage>()
        where TMessageListener : class, IMessageHandlerBase<TMessage>
        where TMessageListenerDefinition : class, IMessageListenerDefinition<TMessageListener>
        where TMessage : class
    {
        var consumer = new ActiveMqConsumer<TMessageListener, TMessage>();
        _serviceCollection.AddScoped<TMessageListener>();
        _serviceCollection.AddSingleton<IMessageListenerDefinition<TMessageListener>, TMessageListenerDefinition>();
        
        DynamicMessageListeners.Add(consumer);
        return this;
    }
    
}