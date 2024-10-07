using Common.DomainEvents;
using Common.IntegrationEvents.Events;
using MediatR;
using Users.Application.Interfaces;
using Users.Domain.DomainEvents;

namespace Users.Application.EventHandlers;

public class BusinessUserUpdatedEventHandler(IUsersRepository repository, IMediator mediator)
    : IDomainEventHandler<BusinessUserUpdatedEvent>
{
    public async Task Handle(BusinessUserUpdatedEvent businessUser, CancellationToken cancellationToken)
    {
        await repository.UpdateBusinessUser(businessUser.User, cancellationToken);
        await mediator.Publish(new UserUpdatedIntegrationEvent
        {
            UserId = businessUser.User.Id,
            Name = businessUser.User.Name
        }, cancellationToken);
    }
}