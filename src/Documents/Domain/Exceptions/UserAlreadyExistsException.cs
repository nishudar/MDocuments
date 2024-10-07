using Common.Abstracts;

namespace Documents.Domain.Exceptions;

public sealed class UserAlreadyExistsException() : BusinessException("User already exists")
{
    public UserAlreadyExistsException(Guid userId) : this()
    {
        Data[nameof(userId)] = userId;
    }
}