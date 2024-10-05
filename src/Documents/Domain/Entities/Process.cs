using Common.Abstracts;
using Documents.Domain.Enums;
using Documents.Domain.Exceptions;
using Documents.Domain.ValueTypes;

namespace Documents.Domain.Entities;

public class Process : Entity, IProcess
{
    public required Guid CustomerId { get; init; }
    public required Guid BusinessUserId { get; init; }
    public required List<Document> Documents { get; init; }
    public required ICollection<DocumentType> AllowedDocumentTypes { get; init; }
    public ProcessStatus Status { get; private set; }

    public bool AllDocumentsProvided()
    {
        return !AllowedDocumentTypes
            .Where(type => type.IsRequired)
            .All(type => Documents.Exists(
                document => document.DocumenType == type.TypeName));
    }

    public void SetStatus(ProcessStatus status)
    {
        if (!AllDocumentsProvided() && status is ProcessStatus.Finished)
            throw new ProcessCannotChangeStatusException("some documents were not provided", Id);

        Status = status;
    }

    public void AddDocument(Document document)
    {
        ValidateDocument(document);
        Documents.Add(document);
    }

    public void ValidateDocument(Document document)
    {
        if (document.UserId != BusinessUserId)
            throw new InvalidUserIdException(document.UserId, document.Id);
        if (document.CustomerId != CustomerId)
            throw new InvalidUserIdException(document.CustomerId, document.Id);
        var allowedDocumentType = AllowedDocumentTypes.FirstOrDefault(type => type.TypeName == document.DocumenType);
        if (allowedDocumentType is null)
            throw new DocumentTypeNotAllowedException(document.DocumenType, Id);
        if (!allowedDocumentType.MultipleAllowed && Documents.Exists(d => d.DocumenType == document.DocumenType))
            throw new DuplicatedDocumentTypeNotAllowedException(document.DocumenType, Id);

    }
}