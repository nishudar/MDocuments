using MediatR;

namespace Common.DomainEvents;

public interface IDomainEventDispatcher
{
    Task DispatchEvents(IEnumerable<INotification> domainEvents, CancellationToken ct = default);
}