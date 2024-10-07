using Common.DomainEvents;
using Documents.Domain.Entities;

namespace Documents.Domain.Events;

public class UserUpdatedEvent : IDomainEvent
{
    public Guid UserId { get; set; }
    public required string UserName { get; init; }
}