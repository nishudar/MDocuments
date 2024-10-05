namespace Documents.Domain.ValueObjects;

public record DocumentType(
    string TypeName,
    bool IsRequired,
    bool MultipleAllowed);