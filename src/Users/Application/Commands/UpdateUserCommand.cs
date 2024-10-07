using Common.DomainEvents;
using MediatR;
using Users.Application.Interfaces;
using Users.Domain.Entities;

namespace Users.Application.Commands;

public record UpdateBusinessUserCommand(Guid UserId, string Name) : IRequest<BusinessUser>;

public class UpdateBusinessUserCommandHandler(
    IDomainEventDispatcher dispatcher,
    IUsersRepository repository)
    : IRequestHandler<UpdateBusinessUserCommand, BusinessUser>
{
    public async Task<BusinessUser> Handle(UpdateBusinessUserCommand command, CancellationToken cancellationToken)
    {
        var usersInventory = await repository.GetUsersInventory(cancellationToken);
        var businessUser = new BusinessUser
        {
            Id = command.UserId,
            Name = command.Name
        };
        await usersInventory.UpdateBusinessUser(command.UserId, businessUser);
        await dispatcher.DispatchEvents(usersInventory.BusinessEvents, cancellationToken);

        return businessUser;
    }
}