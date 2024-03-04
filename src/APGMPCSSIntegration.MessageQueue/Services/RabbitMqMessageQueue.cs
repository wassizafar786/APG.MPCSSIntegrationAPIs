using APG.MessageQueue.Interfaces;
using APG.MessageQueue.MessageDestinations;
using MassTransit;
using MassTransit.Transports.Fabric;

namespace APG.MessageQueue.Services
{
    public class MassTransitMessageBus : IMessageQueue
    {
        private readonly ISendEndpointProvider _sendEndpointProvider;
        private readonly IPublishEndpoint _publishEndpoint;

        public MassTransitMessageBus(ISendEndpointProvider sendEndpointProvider, IPublishEndpoint publishEndpoint)
        {
            _sendEndpointProvider = sendEndpointProvider;
            _publishEndpoint = publishEndpoint;
        }
    
        public async Task SendMessage<T>(T message, IMessageDestination messageDestination, CancellationToken cancellationToken) where T : class
        { 
            var endpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri(messageDestination.ToUri()));
            await endpoint.Send(message, cancellationToken: cancellationToken);
        }
    
        public async Task PublishMessage<T>(T message, CancellationToken cancellationToken) where T : class
        {
            await _publishEndpoint.Publish(message, cancellationToken);
        }


    }    

}
