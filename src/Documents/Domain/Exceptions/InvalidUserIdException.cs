using Common.Abstracts;

namespace Documents.Domain.Exceptions;

public sealed class InvalidUserIdException() : BusinessException("Invalid user id for document")
{
    public InvalidUserIdException(Guid userId, Guid documentId) : this()
    {
        Data[nameof(userId)] = userId;
        Data[nameof(documentId)] = documentId;
    }
}