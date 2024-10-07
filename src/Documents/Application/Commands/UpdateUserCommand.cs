using Common.DomainEvents;
using Documents.Application.Interfaces;
using Documents.Domain.Entities;
using MediatR;

namespace Documents.Application.Commands;

internal record UpdateBusinessUserCommand(Guid UserId, string Name) : IRequest<User>;

internal class UpdateBusinessUserCommandHandler(
    IDomainEventDispatcher dispatcher,
    IDocumentInventoryRepository repository)
    : IRequestHandler<UpdateBusinessUserCommand, User>
{
    public async Task<User> Handle(UpdateBusinessUserCommand command, CancellationToken cancellationToken)
    {
        //Here i'd call user & consumer domain services to verify the users

        var documentInventory = await repository.GetDocumentInventory(cancellationToken);
        
        var user = documentInventory.UpdateUser(command.UserId, command.Name);
        await dispatcher.DispatchEvents(documentInventory.BusinessEvents, cancellationToken);

        return user;
    }
}