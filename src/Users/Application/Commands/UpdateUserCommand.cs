using Common.DomainEvents;
using MediatR;
using Users.Application.Interfaces;
using Users.Domain.Entities;

namespace Users.Application.Commands;

internal record UpdateBusinessUserCommand(Guid UserId, string Name) : IRequest<BusinessUser>;

internal class UpdateBusinessUserCommandHandler(
    IDomainEventDispatcher dispatcher,
    IUsersRepository repository)
    : IRequestHandler<UpdateBusinessUserCommand, BusinessUser>
{
    public async Task<BusinessUser> Handle(UpdateBusinessUserCommand command, CancellationToken cancellationToken)
    {
        var usersInventory = await repository.GetUsersInventory(cancellationToken);
        var businessUser = await usersInventory.UpdateBusinessUser(command.UserId, command.Name);
        await dispatcher.DispatchEvents(usersInventory.BusinessEvents, cancellationToken);

        return businessUser;
    }
}