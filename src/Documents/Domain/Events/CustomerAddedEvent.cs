using Common.DomainEvents;
using Documents.Domain.Entities;

namespace Documents.Domain.Events;

public class CustomerAddedEvent : IDomainEvent
{
    public required Customer Customer { get; init; }
}