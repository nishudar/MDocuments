namespace Common.IntegrationEvents.Events;

public class UserCreatedIntegrationEvent : IIntegrationEvent
{
    public string Topic => IntegrationTopics.UsersTopic;
    
    public required Guid UserId { get; init; }
    public required string Name { get; init; }
    public required string Role { get; init; }
}