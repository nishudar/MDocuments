using Common.DomainEvents;
using Documents.Domain.Entities;

namespace Documents.Domain.Events;

public class BusinessUserUpdatedEvent : IDomainEvent
{
    public required BusinessUser User { get; init; }
}