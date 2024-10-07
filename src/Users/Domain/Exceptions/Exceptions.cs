using Common.Abstracts;

namespace Users.Domain.Exceptions;

public sealed class UserDoesNotExistException() : BusinessException("User does not exist")
{
    public UserDoesNotExistException(Guid userId) : this()
    {
        Data[nameof(userId)] = userId;
    }
}

public sealed class UserElreadyExistsException() : BusinessException("User already exist")
{
    public UserElreadyExistsException(Guid userId) : this()
    {
        Data[nameof(userId)] = userId;
    }
}
