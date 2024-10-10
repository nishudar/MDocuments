using Common.DomainEvents;
using Users.Domain.Entities;

namespace Users.Domain.DomainEvents;

internal class UserAddedEvent : IDomainEvent
{
    public required User User { get; init; }
}