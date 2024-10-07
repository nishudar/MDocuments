using Common.Abstracts;

namespace Documents.Domain.Exceptions;

public class NotInRoleException() : BusinessException($"User has to be in proper role to execute action")
{
    public NotInRoleException(Guid userId, string role, string expectedRole) : this()
    {
        Data[nameof(userId)] = userId;
        Data[nameof(role)] = role;
        Data[nameof(expectedRole)] = expectedRole;
    }
}