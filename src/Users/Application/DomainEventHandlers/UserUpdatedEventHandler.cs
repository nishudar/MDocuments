using Common.DomainEvents;
using Common.IntegrationEvents.Events;
using MediatR;
using Users.Application.Interfaces;
using Users.Domain.DomainEvents;

namespace Users.Application.DomainEventHandlers;

internal class UserUpdatedEventHandler(IUsersUnitOfWork unitOfWork, IMediator mediator)
    : IDomainEventHandler<UserUpdatedEvent>
{
    public async Task Handle(UserUpdatedEvent user, CancellationToken cancellationToken)
    {
        await unitOfWork.UpdateUser(user.User, cancellationToken);
        await mediator.Publish(new UserUpdatedIntegrationEvent
        {
            UserId = user.User.Id,
            Name = user.User.Name
        }, cancellationToken);
    }
}