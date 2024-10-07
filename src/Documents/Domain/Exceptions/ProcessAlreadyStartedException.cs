using Common.Abstracts;

namespace Documents.Domain.Exceptions;

public sealed class ProcessAlreadyStartedException()
    : BusinessException("Cannot start process as it is already started")
{
    public ProcessAlreadyStartedException(Guid customerId, Guid userId, Guid processId) : this()
    {
        Data[nameof(customerId)] = customerId;
        Data[nameof(userId)] = userId;
        Data[nameof(processId)] = processId;
    }
}