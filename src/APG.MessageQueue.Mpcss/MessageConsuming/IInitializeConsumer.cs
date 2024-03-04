using Spring.Messaging.Nms.Connections;

namespace APG.MessageQueue.Mpcss.MessageConsuming;

public interface IInitializeConsumer
{
    public void InitializeConsumer(ActiveMQConsumerConfig activeMQConsumerConfig, CachingConnectionFactory connectionFactory);
}