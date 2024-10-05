using Common.DomainEvents;
using Documents.Application.Interfaces;
using Documents.Domain.Entities;
using MediatR;

namespace Documents.Application.Commands;

public record AssignCustomerCommand(string Name, Guid UserId) : IRequest<Customer>;

public class AssignCustomerCommandHandler(IDomainEventDispatcher dispatcher, IDocumentInventoryRepository repository) 
    : IRequestHandler<AssignCustomerCommand, Customer>
{
    public async Task<Customer> Handle(AssignCustomerCommand command, CancellationToken cancellationToken)
    {
        var documentInventory = await repository.GetDocumentInventory(cancellationToken);
        var id = Guid.NewGuid();
        var customer = new Customer
        {
            Id = id,
            Name = command.Name,
            AssignedUserId = command.UserId
        };
        documentInventory.AssignCustomer(customer);
        await dispatcher.DispatchEvents(documentInventory.BusinessEvents, cancellationToken);
        
        return customer;
    }
}