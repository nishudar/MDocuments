using Common.DomainEvents;
using Documents.Application.Interfaces;
using Documents.Domain.Entities;
using Documents.Infrastructure;
using MediatR;

namespace Documents.Application.Commands;

internal record AddUserCommand(string Name, string Role) : IRequest<User>;

internal class AddUserCommandHandler(IDomainEventDispatcher dispatcher, IDocumentsUnitOfWork unitOfWork)
    : IRequestHandler<AddUserCommand, User>
{
    public async Task<User> Handle(AddUserCommand command, CancellationToken cancellationToken)
    {
        var documentInventory = await unitOfWork.GetDocumentInventory(cancellationToken);
        var id = Guid.NewGuid();
        var user = new User
        {
            Id = id,
            Name = command.Name,
            Role = command.Role
        };
        documentInventory.AddUser(user);
        await dispatcher.DispatchEvents(documentInventory.BusinessEvents, cancellationToken);

        return user;
    }
}