using APG.MessageQueue.Mpcss.ActiveMQTransport;
using APGDigitalIntegration.Common.CommonViewModels.Common;

namespace APGDigitalIntegration.IAL.External.Mpcss.Interfaces
{
    public interface IMpcssCommunicator
    {
        public Task SendMessage<T>(T message, string queue, ActiveMQMessageTypes activeMQMessageType) where T : class;
    }
}
