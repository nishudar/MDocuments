using MediatR;

namespace Common.DomainEvents;

//marker class
public interface IDomainEvent : INotification
{
}