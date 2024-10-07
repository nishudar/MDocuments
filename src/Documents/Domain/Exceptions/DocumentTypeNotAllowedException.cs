using Common.Abstracts;

namespace Documents.Domain.Exceptions;

public sealed class DocumentTypeNotAllowedException() : BusinessException("Document type is not allowed")
{
    public DocumentTypeNotAllowedException(string documentType, Guid processId) : this()
    {
        Data[nameof(documentType)] = documentType;
        Data[nameof(processId)] = processId;
    }
}