using Common.DomainEvents;
using Documents.Application.Interfaces;
using Documents.Domain.Events;

namespace Documents.Application.EventHandlers;

internal class CustomerReassignedEventHandler(IDocumentInventoryRepository repository)
    : IDomainEventHandler<CustomerReassignedEvent>
{
    public async Task Handle(CustomerReassignedEvent domainEvent, CancellationToken cancellationToken)
    {
        await repository.AssignCustomer(domainEvent.Customer, cancellationToken);
    }
}