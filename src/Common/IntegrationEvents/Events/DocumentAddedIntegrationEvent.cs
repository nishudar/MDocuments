namespace Common.IntegrationEvents.Events;

public record DocumentAddedIntegrationEvent(Guid ProcessId, Guid UserId, Guid CustomerId, Guid DocumentId)
    : IIntegrationEvent
{
    public string Topic => IntegrationTopics.DocumentsTopic;
}