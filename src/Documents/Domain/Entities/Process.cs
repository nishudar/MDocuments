using Common.Abstracts;
using Documents.Domain.Enums;
using Documents.Domain.Exceptions;

namespace Documents.Domain.Entities;

public class Process : Entity
{
    public required Guid CustomerId { get; init; }
    public required Guid OperatorUserId { get; init; }
    public required List<Document> Documents { get; init; } = [];
    public List<ProcessDocumentType> AllowedDocumentTypes { get; set; } = [];

    private string _status = ProcessStatus.Unset;
    public string Status
    {
        get => _status;
        set
        {
            if (!ProcessStatus.ProcessStatuses.Contains(value))
                throw new ArgumentException(value);
        
            if (!AllDocumentsProvided() && value is ProcessStatus.Finished)
                throw new ProcessCannotChangeStatusException("some documents were not provided", Id);

            _status = value;
        }
    }

    public bool AllDocumentsProvided()
    {
        return AllowedDocumentTypes
            .Where(type => type.IsRequired)
            .All(type => Documents.Exists(
                document => document.DocumenType == type.TypeName));
    }

    public void AddDocument(Document document)
    {
        ValidateDocument(document);
        Documents.Add(document);
    }

    public void ValidateDocument(Document document)
    {
        if (document.UserId != OperatorUserId)
            throw new InvalidUserIdException(document.UserId, document.Id);
        if (document.CustomerId != CustomerId)
            throw new InvalidUserIdException(document.CustomerId, document.Id);
        var allowedDocumentType = AllowedDocumentTypes.Find(type => type.TypeName == document.DocumenType);
        if (allowedDocumentType is null)
            throw new DocumentTypeNotAllowedException(document.DocumenType, Id);
        if (!allowedDocumentType.MultipleAllowed && Documents.Exists(d => d.DocumenType == document.DocumenType))
            throw new DuplicatedDocumentTypeNotAllowedException(document.DocumenType, Id);
    }
}