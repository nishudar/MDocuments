using Common.DomainEvents;
using Documents.Application.Interfaces;
using Documents.Domain.Entities;
using MediatR;

namespace Documents.Application.Commands;

public record UpdateBusinessUserCommand(Guid UserId, string Name) : IRequest<BusinessUser>;

public class UpdateBusinessUserCommandHandler(IDomainEventDispatcher dispatcher, IDocumentInventoryRepository repository) 
    : IRequestHandler<UpdateBusinessUserCommand, BusinessUser>
{
    public async Task<BusinessUser> Handle(UpdateBusinessUserCommand command, CancellationToken cancellationToken)
    {
        //Here i'd call user & consumer domain services to verify the users
        
        var documentInventory = await repository.GetDocumentInventory(cancellationToken);
        var businessUser = new BusinessUser
        {
            Id = command.UserId,
            Name = command.Name
        };
        documentInventory.SetBusinessUser(businessUser);
        await dispatcher.DispatchEvents(documentInventory.BusinessEvents, cancellationToken);

        return businessUser;
    }
}