namespace APG.MessageQueue.Mpcss.MessageHandling;

public interface IMessageHandlerBase
{
}

public interface IMessageHandlerBase<in TMessage> : IMessageHandlerBase
{
    public Task ProcessMessage(TMessage message);
}