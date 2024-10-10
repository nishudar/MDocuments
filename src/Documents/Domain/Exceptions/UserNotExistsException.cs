using Common.Abstracts;

namespace Documents.Domain.Exceptions;

public sealed class UserNotExistsException() : BusinessException("User does not exist")
{
    public UserNotExistsException(Guid userId) : this()
    {
        Data[nameof(userId)] = userId;
    }
}