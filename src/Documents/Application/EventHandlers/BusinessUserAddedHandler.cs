using Common.DomainEvents;
using Documents.Application.Interfaces;
using Documents.Domain.Events;

namespace Documents.Application.EventHandlers;

internal class BusinessUserAddedHandler(IDocumentInventoryRepository repository)
    : IDomainEventHandler<UserAddedEvent>
{
    public async Task Handle(UserAddedEvent user, CancellationToken cancellationToken)
    {
        await repository.AddUser(user.User, cancellationToken);
    }
}