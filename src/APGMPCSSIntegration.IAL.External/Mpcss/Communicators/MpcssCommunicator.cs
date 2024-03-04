using APG.MessageQueue.Mpcss.ActiveMQTransport;
using APGDigitalIntegration.IAL.External.Mpcss.Interfaces;

namespace APGDigitalIntegration.IAL.External.Mpcss.Communicators;

public class MpcssCommunicator : IMpcssCommunicator
{
    private readonly IActiveMqMessageQueue _messageQueue;

    public MpcssCommunicator(IActiveMqMessageQueue messageQueue)
    {
        _messageQueue = messageQueue;
    }

    public Task SendMessage<T>(T message, string queue, ActiveMQMessageTypes activeMQMessageType)  where T : class
    {
        return _messageQueue.SendMessage(message, queue, activeMQMessageType);
    }
    
}

