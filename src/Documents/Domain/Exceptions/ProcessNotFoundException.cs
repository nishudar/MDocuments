using Common.Abstracts;

namespace Documents.Domain.Exceptions;

public sealed class ProcessNotFoundException() : BusinessException("Process for document not found")
{
    public ProcessNotFoundException(Guid customerId, Guid userId) : this()
    {
        Data[nameof(customerId)] = customerId;
        Data[nameof(userId)] = userId;
    }
    
    public ProcessNotFoundException(Guid processId) : this()
    {
        Data[nameof(processId)] = processId;
    }
}

