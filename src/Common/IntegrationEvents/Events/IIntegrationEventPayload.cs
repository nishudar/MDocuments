using MediatR;

namespace Common.IntegrationEvents.Events;

public interface IIntegrationEvent : INotification
{
    string Topic { get; }
}