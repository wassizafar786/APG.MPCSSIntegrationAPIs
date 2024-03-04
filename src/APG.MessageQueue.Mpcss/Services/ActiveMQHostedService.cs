using System.Collections.ObjectModel;
using Apache.NMS;
using Apache.NMS.ActiveMQ;
using APG.MessageQueue.Mpcss.Configurator;
using APG.MessageQueue.Mpcss.Interfaces;
using APG.MessageQueue.Mpcss.MessageConsuming;
using APG.MessageQueue.Mpcss.MessageHandling;
using APG.MessageQueue.Mpcss.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Polly;
using Spring.Messaging.Nms.Connections;
using Spring.Messaging.Nms.Core;

namespace APG.MessageQueue.Mpcss.Services;

public class ActiveMQHostedService :BackgroundService, IActiveMQHostedService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ActiveMQConfigurator _activeMQConfigurator;
    private readonly ActiveMqConfiguration _activeMqConfiguration;
    private readonly HeartBeatStatus _heartbeatData;
    private Collection<IMessageListener> ActiveMQConsumers { get; set; }

    CachingConnectionFactory connectionFactory = null;

    public ActiveMQHostedService(IOptions<ActiveMqConfiguration> activeMqConfiguration, IServiceScopeFactory serviceScopeFactory, ActiveMQConfigurator activeMQConfigurator, HeartBeatStatus heartbeatData)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _activeMQConfigurator = activeMQConfigurator;
        _activeMqConfiguration = activeMqConfiguration.Value;

        ActiveMQConsumers = new Collection<IMessageListener>();
        _heartbeatData = heartbeatData;
    }
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _heartbeatData.ConnectionIsProcessing = true;

        Task.Run(() =>
        {
            Policy
                .Handle<NMSConnectionException>()
                .WaitAndRetryForever(s => TimeSpan.FromSeconds(_activeMqConfiguration.ConnectionRecoveryIntervalInSeconds),
                    onRetry: (_, _) => { ClearConsumers(); })
                .Execute(InitializeActiveMQConsumers);
        }, stoppingToken);

        return Task.CompletedTask;
    }

    private void InitializeActiveMQConsumers()
    {
        var scope = _serviceScopeFactory.CreateScope();

        connectionFactory = new CachingConnectionFactory(new ConnectionFactory(_activeMqConfiguration.ActiveMqUrl));

        foreach (var messageListener in _activeMQConfigurator.DynamicMessageListeners)
        {
            var messageType = messageListener.GetType().GetGenericArguments()[0];
            var myType = typeof(IMessageListenerDefinition<>).MakeGenericType(messageType);
            var messageListenerDefinition = scope.ServiceProvider.GetService(myType) as IMessageListenerDefinition;

            var configs = new ActiveMQConsumerConfig()
            {
                ActiveMQUri = _activeMqConfiguration.ActiveMqUrl,
                ServiceScopeFactory = _serviceScopeFactory,
                DestinationQueue = messageListenerDefinition.DestinationQueue,
                ConnectionRecoveryIntervalInSeconds = _activeMqConfiguration.ConnectionRecoveryIntervalInSeconds
            };

            ((IInitializeConsumer)messageListener).InitializeConsumer(configs, connectionFactory);
            ActiveMQConsumers.Add(messageListener);
        }
        _heartbeatData.ConnectionIsProcessing = false;
    }


    private void ClearConsumers()
    {
        foreach (IDisposable activeMQConsumer in ActiveMQConsumers)
        {
            try
            {
                activeMQConsumer.Dispose();
            }
            catch
            {
                // ignored
            }
        }

        ActiveMQConsumers.Clear();

    }

    public async Task CallExceuteAsync(CancellationToken stoppingToken)
    {
        ClearConsumers();
        await ExecuteAsync(stoppingToken);
    }


    public override void Dispose()
    {
        ClearConsumers();

        base.Dispose();
    }


}