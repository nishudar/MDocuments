using Common.DomainEvents;
using Documents.Application.Interfaces;
using Documents.Domain.Events;

namespace Documents.Application.EventHandlers;

public class CustomerAddedEventHandler(IDocumentInventoryRepository repository)
    : IDomainEventHandler<CustomerAddedEvent>
{
    public async Task Handle(CustomerAddedEvent domainEvent, CancellationToken cancellationToken)
    {
        await repository.AddCustomer(domainEvent.Customer, cancellationToken);
    }
}