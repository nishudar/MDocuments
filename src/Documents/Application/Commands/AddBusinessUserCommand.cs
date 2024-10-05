using Common.DomainEvents;
using Documents.Application.Interfaces;
using Documents.Domain.Entities;
using MediatR;

namespace Documents.Application.Commands;

public record AddBusinessUserCommand(string Name) : IRequest<BusinessUser>;

public class AddBusinessUserCommandHandler(IDomainEventDispatcher dispatcher, IDocumentInventoryRepository repository) 
    : IRequestHandler<AddBusinessUserCommand, BusinessUser>
{
    public async Task<BusinessUser> Handle(AddBusinessUserCommand command, CancellationToken cancellationToken)
    {
        //Here i'd call user & consumer domain services to verify the users
        
        var documentInventory = await repository.GetDocumentInventory(cancellationToken);
        var id = Guid.NewGuid();
        var businessUser = new BusinessUser
        {
            Id = id,
            Name = command.Name
        };
        documentInventory.SetBusinessUser(businessUser);
        await dispatcher.DispatchEvents(documentInventory.BusinessEvents, cancellationToken);

        return businessUser;
    }
}