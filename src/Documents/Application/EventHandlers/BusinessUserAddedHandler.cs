using Common.DomainEvents;
using Documents.Application.Interfaces;
using Documents.Domain.Events;

namespace Documents.Application.EventHandlers;

internal class BusinessUserAddedHandler(IDocumentInventoryRepository repository)
    : IDomainEventHandler<BusinessUserAddedEvent>
{
    public async Task Handle(BusinessUserAddedEvent user, CancellationToken cancellationToken)
    {
        await repository.AddBusinessUser(user.User, cancellationToken);
    }
}