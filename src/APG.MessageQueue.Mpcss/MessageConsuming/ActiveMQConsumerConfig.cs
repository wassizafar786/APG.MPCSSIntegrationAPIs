using Microsoft.Extensions.DependencyInjection;

namespace APG.MessageQueue.Mpcss.MessageConsuming;

public class ActiveMQConsumerConfig
{
    public string ActiveMQUri { get; set; }
    public string DestinationQueue { get; set; }
    public int ConnectionRecoveryIntervalInSeconds { get; set; }
    public IServiceScopeFactory ServiceScopeFactory { get; set; }
}