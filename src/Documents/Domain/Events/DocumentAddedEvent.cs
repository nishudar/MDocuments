using Common.DomainEvents;
using Documents.Domain.Entities;

namespace Documents.Domain.Events;

public class DocumentAddedEvent : IDomainEvent
{
    public required Document Document { get; init; }
}