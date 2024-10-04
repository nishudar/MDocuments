using Common.DomainEvents;
using Documents.Domain.ValueTypes;

namespace Documents.Domain.Events;

public class ProcessReportGeneratedEvent : IDomainEvent
{
    public required Guid ProcessId { get; init; }
    public required ProcessReport ProcessReport { get; init; }
}