using Common.DomainEvents;
using Documents.Application.Interfaces;
using Documents.Domain.Events;

namespace Documents.Application.EventHandlers;

public class BusinessUserUpdatedEventHandler(IDocumentInventoryRepository repository) : IDomainEventHandler<BusinessUserUpdatedEvent>
{
    public async Task Handle(BusinessUserUpdatedEvent businessUser, CancellationToken cancellationToken)
    {
        await repository.UpdateBusinessUser(businessUser.User, cancellationToken);
    }
}