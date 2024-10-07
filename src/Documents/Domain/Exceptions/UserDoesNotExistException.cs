using Common.Abstracts;

namespace Documents.Domain.Exceptions;

public sealed class UserDoesNotExistException() : BusinessException("User does not exist")
{
    public UserDoesNotExistException(Guid userId) : this()
    {
        Data[nameof(userId)] = userId;
    }
}