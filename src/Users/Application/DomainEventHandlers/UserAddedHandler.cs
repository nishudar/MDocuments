using Common.DomainEvents;
using Common.IntegrationEvents.Events;
using MediatR;
using Users.Application.Interfaces;
using Users.Domain.DomainEvents;

namespace Users.Application.DomainEventHandlers;

internal class UserAddedHandler(IUsersUnitOfWork unitOfWork, IMediator mediator)
    : IDomainEventHandler<UserAddedEvent>
{
    public async Task Handle(UserAddedEvent user, CancellationToken cancellationToken)
    {
        await unitOfWork.AddUser(user.User, cancellationToken);
        await unitOfWork.SaveChanges(cancellationToken);
        await mediator.Publish(new UserCreatedIntegrationEvent
        {
            UserId = user.User.Id,
            Name = user.User.Name,
            Role = user.User.Role
        }, cancellationToken);
    }
}