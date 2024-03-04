using APG.MessageQueue.MessageDestinations;

namespace APG.MessageQueue.Interfaces;

public interface IMessageQueue
{
    Task SendMessage<T>(T message, IMessageDestination messageDestination, CancellationToken cancellationToken) where T : class;
    Task PublishMessage<T>(T message, CancellationToken cancellationToken) where T : class;
}