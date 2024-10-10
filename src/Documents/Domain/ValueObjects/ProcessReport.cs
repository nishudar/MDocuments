namespace Documents.Domain.ValueObjects;

public record ProcessReport(
    Guid ProcessId,
    Guid? UserId,
    Guid? CustomerId,
    string UserName,
    string CustomerName,
    string?[] RequiredDocuments,
    IReadOnlyCollection<DocumentWithType> ProvidedDocuments
);

public record DocumentWithType(string documentName, string documentType);