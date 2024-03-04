namespace APG.MessageQueue.Mpcss.MessageHandling;

public interface IMessageListenerDefinition<in TListener> : IMessageListenerDefinition
{
}

public interface IMessageListenerDefinition
{
    public string DestinationQueue { get; }
}