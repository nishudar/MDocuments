using Common.Abstracts;

namespace Users.Domain.Exceptions;

internal sealed class UserDoesNotExistException() : BusinessException("User does not exist")
{
    public UserDoesNotExistException(Guid userId) : this()
    {
        Data[nameof(userId)] = userId;
    }
}