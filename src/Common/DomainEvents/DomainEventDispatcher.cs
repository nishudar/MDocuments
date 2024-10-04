using MediatR;
using Microsoft.Extensions.Logging;

namespace Common.DomainEvents;

using System.Threading;
using System.Threading.Tasks;

public class DomainEventDispatcher(IMediator mediator, ILogger<DomainEventDispatcher> logger) : IDomainEventDispatcher
{
    private readonly SemaphoreSlim _semaphoreSlim = new(1, 1);
    public async Task DispatchEvents(IEnumerable<INotification> domainEvents, CancellationToken ct = default)
    {
        await _semaphoreSlim.WaitAsync(ct);
        try
        {
            foreach (var domainEvent in domainEvents)
            {
                logger.LogInformation("Handling domain event {Type}", domainEvent.GetType().Name);
                await mediator.Publish(domainEvent, ct);
                logger.LogDebug("Handled domain event {Type}", domainEvent.GetType().Name);
            }
        }
        finally
        {
            _semaphoreSlim.Release();
        }
    }
}