using Common.DomainEvents;
using Common.IntegrationEvents.Events;
using MediatR;
using Users.Application.Interfaces;
using Users.Domain.DomainEvents;

namespace Users.Application.EventHandlers;

internal class BusinessUserAddedHandler(IUsersRepository repository, IMediator mediator)
    : IDomainEventHandler<BusinessUserAddedEvent>
{
    public async Task Handle(BusinessUserAddedEvent user, CancellationToken cancellationToken)
    {
        await repository.AddBusinessUser(user.User, cancellationToken);
        await mediator.Publish(new UserCreatedIntegrationEvent()
        {
            UserId = user.User.Id,
            Name = user.User.Name,
            Role = user.User.Role
        }, cancellationToken);
    }
}