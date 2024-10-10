using Common.DomainEvents;
using Documents.Application.Interfaces;
using Documents.Domain.Entities;
using Documents.Infrastructure;
using MediatR;

namespace Documents.Application.Commands;

internal record UpdateUserCommand(Guid UserId, string Name) : IRequest<User>;

internal class UpdateUserCommandHandler(
    IDomainEventDispatcher dispatcher,
    IDocumentsUnitOfWork unitOfWork)
    : IRequestHandler<UpdateUserCommand, User>
{
    public async Task<User> Handle(UpdateUserCommand command, CancellationToken cancellationToken)
    {
        //Here i'd call user & consumer domain services to verify the users

        var documentInventory = await unitOfWork.GetDocumentInventory(cancellationToken);
        
        var user = documentInventory.UpdateUser(command.UserId, command.Name);
        await dispatcher.DispatchEvents(documentInventory.BusinessEvents, cancellationToken);

        return user;
    }
}