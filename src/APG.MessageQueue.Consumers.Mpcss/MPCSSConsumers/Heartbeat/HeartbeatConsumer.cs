using APG.MessageQueue.Mpcss;
using APG.MessageQueue.Mpcss.MessageHandling;
using APGDigitalIntegration.Common.CommonViewModels.Heartbeat;
using APGDigitalIntegration.Constant;

namespace APG.MessageQueue.Consumers.Mpcss.MPCSSConsumers.Heartbeat;

public class HeartbeatConsumer : IMessageHandlerBase<HeartBeatMessage>
{
    public HeartBeatStatus _heartbeatData { get; set; }
    public HeartbeatConsumer(HeartBeatStatus heartbeatData)
    {
        _heartbeatData = heartbeatData;
    }

    public Task ProcessMessage(HeartBeatMessage heartBeatMessage)
    {
        _heartbeatData.LastHeartBeatReceived = DateTime.UtcNow;
        return Task.CompletedTask;
    }
}

public class HeartbeatConsumerDefinition : IMessageListenerDefinition<HeartbeatConsumer>
{
    public string DestinationQueue => MPCSSQueues.HeartBeatResponseQueue;
}
