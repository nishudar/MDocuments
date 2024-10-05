using Common.DomainEvents;
using Documents.Api.Models;
using Documents.Application.Interfaces;
using Documents.Domain.Entities;
using MediatR;

namespace Documents.Application.Commands;

public record UploadDocumentCommand(DocumentUploadModel UploadedModel) : IRequest<Guid>;

public class UploadDocumentHnandler(
    IDomainEventDispatcher dispatcher, 
    IDocumentInventoryRepository repository,
    IStorageService storageService)
    : IRequestHandler<UploadDocumentCommand, Guid>
{
    public async Task<Guid> Handle(UploadDocumentCommand request, CancellationToken cancellationToken)
    {
        var upload = request.UploadedModel;
        var documentInventory = await repository.GetDocumentInventory(cancellationToken);
        var document = new Document
        {
            Id = Guid.NewGuid(),
            Name = upload.FileName,
            UserId = upload.UserId,
            CustomerId = upload.CustomerId,
            DocumenType = upload.DocumentType,
        };
        documentInventory.ValidateDocument(document);
        var uploadResponse = await storageService.UploadFile(
            upload.File.OpenReadStream(),
            upload.FileName,
            upload.DocumentType,
            upload.UserId,
            cancellationToken
        );
        document.SetFileId(uploadResponse.FileId);
        documentInventory.AddDocument(document);
        await dispatcher.DispatchEvents(documentInventory.BusinessEvents, cancellationToken);
        
        return document.Id;
    }
}