using Common.Abstracts;

namespace Users.Domain.Exceptions;

internal sealed class UserAlreadyExistsException() : BusinessException("User already exist")
{
    public UserAlreadyExistsException(Guid userId) : this()
    {
        Data[nameof(userId)] = userId;
    }
}
