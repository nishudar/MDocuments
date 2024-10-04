using Common.Abstracts;

namespace Common.IntegrationEvents.Events;

public record ProcessStatusUpdate(Guid ProcessId, Guid UserId, Guid CustomerId, ProcessStatus ProcessStatus) : IIntegrationEvent;

