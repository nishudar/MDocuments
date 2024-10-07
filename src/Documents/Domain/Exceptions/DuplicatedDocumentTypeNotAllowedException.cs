using Common.Abstracts;

namespace Documents.Domain.Exceptions;

public sealed class DuplicatedDocumentTypeNotAllowedException()
    : BusinessException("Duplicated document type is not allowed")
{
    public DuplicatedDocumentTypeNotAllowedException(string documentType, Guid processId) : this()
    {
        Data[nameof(documentType)] = documentType;
        Data[nameof(processId)] = processId;
    }
}