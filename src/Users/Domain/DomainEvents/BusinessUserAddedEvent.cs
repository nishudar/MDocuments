using Common.DomainEvents;
using Users.Domain.Entities;

namespace Users.Domain.DomainEvents;

internal class BusinessUserAddedEvent : IDomainEvent
{
    public required BusinessUser User { get; init; }
}