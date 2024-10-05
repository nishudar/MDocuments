namespace Common.IntegrationEvents.Events;

public record ProcessStatusUpdateIntegrationEvent(Guid ProcessId, Guid UserId, Guid CustomerId, string ProcessStatus)
    : IIntegrationEvent
{
    public string Topic => IntegrationTopics.DocumentsTopic;
}

