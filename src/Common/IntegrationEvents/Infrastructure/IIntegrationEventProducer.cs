using Common.IntegrationEvents.Events;

namespace Common.IntegrationEvents.Infrastructure;

public interface IIntegrationEventProducer
{
    Task Publish<T>(T eventObject, string key, string topic, CancellationToken cancellationToken)
        where T : IIntegrationEvent;

    Task Publish<T>(T eventObject, string key, string eventType, string topic, CancellationToken cancellationToken)
        where T : IIntegrationEvent?;
}