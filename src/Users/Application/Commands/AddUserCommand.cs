using Common.DomainEvents;
using MediatR;
using Users.Application.Interfaces;
using Users.Domain.Entities;
using Users.Infrastructure;

namespace Users.Application.Commands;

internal record AddUserCommand(string Name, string Role) : IRequest<User>;

internal class AddUserCommandHandler(IDomainEventDispatcher dispatcher, IUsersUnitOfWork unitOfWork)
    : IRequestHandler<AddUserCommand, User>
{
    public async Task<User> Handle(AddUserCommand command, CancellationToken cancellationToken)
    {
        var usersInventory = await unitOfWork.GetUsersInventory(cancellationToken);
        var user = await usersInventory.AddUser(command.Name, command.Role);
        await dispatcher.DispatchEvents(usersInventory.BusinessEvents, cancellationToken);

        return user;
    }
}