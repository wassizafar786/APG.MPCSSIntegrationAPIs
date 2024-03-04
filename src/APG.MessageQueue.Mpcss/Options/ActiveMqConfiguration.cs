namespace APG.MessageQueue.Mpcss.Options
{
    public class ActiveMqConfiguration
    {
        public string ActiveMqUrl { get; set; }
        
        public string UserName { get; set; }
        
        public string Password { get; set; }
        public int ConnectionRecoveryIntervalInSeconds { get; set; }
        
        public PublishRetryPolicy PublishRetryPolicy { get; set; }
        public HeartBeat HeartBeat { get; set; }
    }

    public class PublishRetryPolicy
    {
        public bool IsEnabled { get; set; }
        public int NumberOfRetries { get; set; }
        public int TimeInBetweenRetriesInMillieSeconds { get; set; }
    }
    
    public class HeartBeat
    {
        public bool IsEnabled { get; set; }
        public int HeartBeatIntervalInSeconds { get; set; }
    }
}
