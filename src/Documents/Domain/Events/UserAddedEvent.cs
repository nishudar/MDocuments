using Common.DomainEvents;
using Documents.Domain.Entities;

namespace Documents.Domain.Events;

public class UserAddedEvent : IDomainEvent
{
    public required User User { get; init; }
}