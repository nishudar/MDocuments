using Common.DomainEvents;
using Documents.Domain.Entities;

namespace Documents.Domain.Events;

public class 
    ProcessChangedStatusEvent : IDomainEvent
{
    public required Process Process { get; init; }
}