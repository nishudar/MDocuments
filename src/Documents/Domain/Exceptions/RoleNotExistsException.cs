using Common.Abstracts;

namespace Documents.Domain.Exceptions;

public class RoleNotExistsException() : BusinessException($"Role not exists")
{
    public RoleNotExistsException(string role) : this()
    {
        Data[nameof(role)] = role;
    }
}