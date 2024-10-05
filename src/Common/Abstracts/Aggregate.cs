using Common.DomainEvents;

namespace Common.Abstracts;

public interface IAggregate
{
    Guid Id { get; init; }

    ICollection<IDomainEvent> BusinessEvents { get; }
}

public abstract class Aggregate : IAggregate
{
    private readonly List<IDomainEvent> _businessEvents = [];
    public Guid Id { get; init; }
    public ICollection<IDomainEvent> BusinessEvents => _businessEvents.ToArray();

    protected void AddBusinessEvent(IDomainEvent businessEvent)
    {
        _businessEvents.Add(businessEvent);
    }
}