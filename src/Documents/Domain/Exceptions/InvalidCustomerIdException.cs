using Common.Abstracts;

namespace Documents.Domain.Exceptions;

public sealed class InvalidCustomerIdException() : BusinessException("Invalid user id for document")
{
    public InvalidCustomerIdException(Guid customerId, Guid documentId) : this()
    {
        Data[nameof(customerId)] = customerId;
        Data[nameof(documentId)] = documentId;
    }
}