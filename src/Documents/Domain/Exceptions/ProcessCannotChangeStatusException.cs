using Common.Abstracts;

namespace Documents.Domain.Exceptions;

public sealed class ProcessCannotChangeStatusException(string reason)
    : BusinessException($"Cannot finish process: {reason}")
{
    public ProcessCannotChangeStatusException(string reason, Guid processId) : this(reason)
    {
        Data[nameof(processId)] = processId;
    }
}