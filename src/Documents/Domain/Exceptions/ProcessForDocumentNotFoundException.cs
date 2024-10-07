using Common.Abstracts;

namespace Documents.Domain.Exceptions;

public sealed class ProcessForDocumentNotFoundException() : BusinessException("Process for document not found")
{
    public ProcessForDocumentNotFoundException(Guid customerId, Guid userId) : this()
    {
        Data[nameof(customerId)] = customerId;
        Data[nameof(userId)] = userId;
    }
}