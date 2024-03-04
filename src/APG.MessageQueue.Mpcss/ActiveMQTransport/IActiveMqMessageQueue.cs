using Apache.NMS;
using Apache.NMS.ActiveMQ;
using Apache.NMS.ActiveMQ.Commands;
using APG.MessageQueue.Mpcss.Options;
using APGDigitalIntegration.DomainHelper.Services;
using Microsoft.Extensions.Options;
using Polly;
using Spring.Messaging.Nms.Connections;

namespace APG.MessageQueue.Mpcss.ActiveMQTransport;

public interface IActiveMqMessageQueue
{
    public Task SendMessage<T>(T message, string queue, ActiveMQMessageTypes activeMQMessageType) where T : class;
}

public class NewActiveMQMessageQueue : IActiveMqMessageQueue, IDisposable
{
    private readonly CachingConnectionFactory _connectionFactory;
    private readonly IConnection _cachedConnection;
    private readonly IAsyncPolicy _policy;


    public NewActiveMQMessageQueue(IOptions<ActiveMqConfiguration> activeMQConfigs)
    {
        var activeMqConfigs = activeMQConfigs.Value;

        var connFactoryInstance = new ConnectionFactory(new Uri(activeMqConfigs.ActiveMqUrl));
        connFactoryInstance.Password=activeMQConfigs.Value.Password;

        _connectionFactory = new CachingConnectionFactory(connFactoryInstance)
        {
            
        };
        this._cachedConnection = _connectionFactory.CreateConnection();
        
        if (activeMqConfigs.PublishRetryPolicy.IsEnabled == false)
            _policy = Policy.NoOpAsync();
        
        _policy = Policy.Handle<NMSException>()
            .WaitAndRetryAsync(activeMqConfigs.PublishRetryPolicy.NumberOfRetries, _ => TimeSpan.FromMilliseconds(activeMqConfigs.PublishRetryPolicy.TimeInBetweenRetriesInMillieSeconds));
    }

    public async Task SendMessage<T>(T message, string queue, ActiveMQMessageTypes activeMQMessageType) where T : class
    {
        await _policy.ExecuteAsync(async () => await SendMessageCore(message, queue, activeMQMessageType));
    }

    private async Task SendMessageCore<T>(T message, string queue, ActiveMQMessageTypes activeMQMessageType) where T : class
    {
        using var cachedSession = await _connectionFactory.GetSessionAsync(_cachedConnection, AcknowledgementMode.AutoAcknowledge);
        using var cachedProducer = await cachedSession.CreateProducerAsync(new ActiveMQQueue(queue));
        cachedProducer.DeliveryMode = MsgDeliveryMode.Persistent;

        if (activeMQMessageType is ActiveMQMessageTypes.Text)
        {
            var serializedMessage = message.ToXml();
            var stringMessage = await cachedProducer.CreateTextMessageAsync(serializedMessage);
            await cachedProducer.SendAsync(stringMessage);
        }
        else
        {
            throw new NotImplementedException("Only ActiveMQMessageTypes.Text Messages are currently accepted");
        }
    }

    public void Dispose()
    {
        _cachedConnection?.Dispose();
        _connectionFactory?.Dispose();
    }
}

public enum ActiveMQMessageTypes
{
    Text,
    Bytes
}