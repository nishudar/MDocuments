using Common.Abstracts;

namespace Documents.Domain.Entities;

public class DocumentType : Entity
{
    public required string TypeName { get; init; }
    public required bool IsRequired { get; init; }
    public required bool MultipleAllowed { get; init; }
}