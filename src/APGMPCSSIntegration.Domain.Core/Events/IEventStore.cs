using NetDevPack.Messaging;

namespace APGDigitalIntegration.Domain.Core.Events
{
    public interface IEventStore
    {
        void Save<T>(T theEvent) where T : Event;
    }
}