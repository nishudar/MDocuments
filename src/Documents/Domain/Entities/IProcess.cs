using Documents.Domain.Enums;
using Documents.Domain.ValueObjects;

namespace Documents.Domain.Entities;

public interface IProcess
{
    Guid CustomerId { get; init; }
    Guid BusinessUserId { get; init; }
    List<Document> Documents { get; init; }
    ICollection<DocumentType> AllowedDocumentTypes { get; init; }
    ProcessStatus Status { get; }
    Guid Id { get; set; }
    bool AllDocumentsProvided();
    void SetStatus(ProcessStatus status);
    void AddDocument(Document document);
    void ValidateDocument(Document document);
}