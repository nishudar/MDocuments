using Common.Abstracts;

namespace Common.IntegrationEvents.Events;

public record ProcessStatusUpdateIntegrationEvent(Guid ProcessId, Guid UserId, Guid CustomerId, ProcessStatus ProcessStatus)
    : IIntegrationEvent
{
    public string Topic => IntegrationTopics.DocumentsTopic;
}

