namespace Common.IntegrationEvents.Events;

public class UserUpdatedIntegrationEvent : IIntegrationEvent
{
    public string Topic => IntegrationTopics.UsersTopic;
    
    public required Guid UserId { get; init; }
    public required string Name { get; init; }
}