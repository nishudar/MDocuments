using Common.DomainEvents;
using Documents.Application.Interfaces;
using Documents.Domain.Events;

namespace Documents.Application.DomainEventHandlers;

internal class UserUpdatedEventHandler(IDocumentsUnitOfWork unitOfWork)
    : IDomainEventHandler<UserUpdatedEvent>
{
    public async Task Handle(UserUpdatedEvent @event, CancellationToken cancellationToken)
    {
        await unitOfWork.UpdateUser(@event.UserId, @event.UserName, cancellationToken);
        await unitOfWork.SaveChanges(cancellationToken);
    }
}