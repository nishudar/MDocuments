using Common.DomainEvents;

namespace Common.Abstracts;

public abstract class Aggregate
{
    public Guid Id { get; init; }
    public readonly List<IDomainEvent> BusinessEvents = new();
}