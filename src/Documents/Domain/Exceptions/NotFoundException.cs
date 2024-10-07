using Common.Abstracts;

namespace Documents.Domain.Exceptions;

public sealed class NotFoundException(string entity) : BusinessException($"Entity not found - id: {entity}")
{
    public NotFoundException(string entity, Guid id) : this(entity)
    {
        Data[nameof(entity)] = entity;
        Data[nameof(id)] = id;
    }
}