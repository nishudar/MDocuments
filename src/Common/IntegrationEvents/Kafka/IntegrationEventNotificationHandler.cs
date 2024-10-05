using Common.IntegrationEvents.Events;
using MediatR;

namespace Common.IntegrationEvents.Kafka;

public class IntegrationEventNotificationHandler<T>(IIntegrationEventProducer producer)
    : INotificationHandler<T> where T: IIntegrationEvent
{
    public async Task Handle(T notification, CancellationToken cancellationToken)
    {
        var key = Guid.NewGuid().ToString();
        var eventType = notification.GetType().FullName!;
        var topic = notification.Topic;
		
        await producer.Publish(notification, key, eventType, topic, cancellationToken);
    }
}