using Common.DomainEvents;
using Users.Domain.Entities;

namespace Users.Domain.DomainEvents;

internal class UserUpdatedEvent : IDomainEvent
{
    public required User User { get; init; }
}