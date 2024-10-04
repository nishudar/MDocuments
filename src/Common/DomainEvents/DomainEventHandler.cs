using MediatR;

namespace Common.DomainEvents;

public interface IDomainEventHandler<in TEventType> : INotificationHandler<TEventType> where TEventType: IDomainEvent { }