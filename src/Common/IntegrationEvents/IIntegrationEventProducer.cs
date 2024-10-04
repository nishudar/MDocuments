using Common.IntegrationEvents.Events;

namespace Common.IntegrationEvents;

public interface IIntegrationEventProducer
{
    public Task SendEvent<T>(string topic, string key, T eventObject, CancellationToken cancellationToken) where T : IIntegrationEvent;
}