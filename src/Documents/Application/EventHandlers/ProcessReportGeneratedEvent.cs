using Common.DomainEvents;
using Documents.Domain.Events;

namespace Documents.Application.EventHandlers;

public class ProcessReportGeneratedEventHandler(ILogger<ProcessReportGeneratedEventHandler> logger)
    : IDomainEventHandler<ProcessReportGeneratedEvent>
{
    public Task Handle(ProcessReportGeneratedEvent domainEvent, CancellationToken cancellationToken)
    {
        logger.LogInformation("Generated process report for {ProcessId}", domainEvent.ProcessId);

        return Task.CompletedTask;
    }
}