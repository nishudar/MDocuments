namespace Documents.Domain.ValueTypes;

public record DocumentType(
    string TypeName,
    bool IsRequired, 
    bool MultipleAllowed);