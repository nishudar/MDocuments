using Common.DomainEvents;
using Documents.Application.Interfaces;
using Documents.Domain.Events;

namespace Documents.Application.EventHandlers;

internal class BusinessUserUpdatedEventHandler(IDocumentInventoryRepository repository)
    : IDomainEventHandler<UserUpdatedEvent>
{
    public async Task Handle(UserUpdatedEvent @event, CancellationToken cancellationToken)
    {
        await repository.UpdateBusinessUser(@event.UserId, @event.UserName, cancellationToken);
    }
}