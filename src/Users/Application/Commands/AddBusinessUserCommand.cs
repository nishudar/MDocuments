using Common.DomainEvents;
using MediatR;
using Users.Application.Interfaces;
using Users.Domain.Entities;

namespace Users.Application.Commands;

public record AddBusinessUserCommand(string Name) : IRequest<BusinessUser>;

public class AddBusinessUserCommandHandler(IDomainEventDispatcher dispatcher, IUsersRepository repository)
    : IRequestHandler<AddBusinessUserCommand, BusinessUser>
{
    public async Task<BusinessUser> Handle(AddBusinessUserCommand command, CancellationToken cancellationToken)
    {
        var usersInventory = await repository.GetUsersInventory(cancellationToken);
        var businessUser = await usersInventory.AddBusinessUser(command.Name);
        await dispatcher.DispatchEvents(usersInventory.BusinessEvents, cancellationToken);

        return businessUser;
    }
}