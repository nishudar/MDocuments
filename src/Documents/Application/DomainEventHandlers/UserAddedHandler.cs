using Common.DomainEvents;
using Documents.Application.Interfaces;
using Documents.Domain.Events;

namespace Documents.Application.DomainEventHandlers;

internal class UserAddedHandler(IDocumentsUnitOfWork unitOfWork)
    : IDomainEventHandler<UserAddedEvent>
{
    public async Task Handle(UserAddedEvent user, CancellationToken cancellationToken)
    {
        unitOfWork.AddUser(user.User);
        await unitOfWork.SaveChanges(cancellationToken);
    }
}