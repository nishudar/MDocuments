using Common.DomainEvents;
using Users.Domain.Entities;

namespace Users.Domain.DomainEvents;

public class BusinessUserUpdatedEvent : IDomainEvent
{
    public required BusinessUser User { get; init; }
}