using Common.DomainEvents;
using Documents.Application.Interfaces;
using Documents.Domain.Entities;
using MediatR;

namespace Documents.Application.Commands;

internal record AddBusinessUserCommand(string Name, string Role) : IRequest<User>;

internal class AddBusinessUserCommandHandler(IDomainEventDispatcher dispatcher, IDocumentInventoryRepository repository)
    : IRequestHandler<AddBusinessUserCommand, User>
{
    public async Task<User> Handle(AddBusinessUserCommand command, CancellationToken cancellationToken)
    {
        var documentInventory = await repository.GetDocumentInventory(cancellationToken);
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