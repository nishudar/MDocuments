using Common.DomainEvents;
using MediatR;
using Users.Application.Interfaces;
using Users.Domain.Entities;
using Users.Infrastructure;

namespace Users.Application.Commands;

internal record UpdateUserCommand(Guid UserId, string Name) : IRequest<User>;

internal class UpdateUserCommandHandler(
    IDomainEventDispatcher dispatcher,
    IUsersUnitOfWork unitOfWork)
    : IRequestHandler<UpdateUserCommand, User>
{
    public async Task<User> Handle(UpdateUserCommand command, CancellationToken cancellationToken)
    {
        var usersInventory = await unitOfWork.GetUsersInventory(cancellationToken);
        var user = await usersInventory.UpdateUser(command.UserId, command.Name);
        await dispatcher.DispatchEvents(usersInventory.BusinessEvents, cancellationToken);

        return user;
    }
}