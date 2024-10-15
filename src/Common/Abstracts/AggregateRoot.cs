using Common.DomainEvents;

namespace Common.Abstracts;

public interface IAggregateRoot
{
    Guid Id { get; init; }

    ICollection<IDomainEvent> BusinessEvents { get; }
}

public abstract class AggregateRootRoot : IAggregateRoot
{
    private readonly List<IDomainEvent> _businessEvents = [];
    public Guid Id { get; init; }
    public ICollection<IDomainEvent> BusinessEvents => _businessEvents.ToArray();

    protected void PublishBusinessEvent(IDomainEvent businessEvent)
    {
        _businessEvents.Add(businessEvent);
    }
}