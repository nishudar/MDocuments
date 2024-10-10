using Common.DomainEvents;
using Documents.Api.Models;
using Documents.Application.Interfaces;
using Documents.Domain.Entities;
using Documents.Infrastructure;
using MediatR;

namespace Documents.Application.Commands;

internal record UploadDocumentCommand(DocumentUploadModel UploadedModel) : IRequest<Guid>;

internal class UploadDocumentHnandler(
    IDomainEventDispatcher dispatcher,
    IDocumentsUnitOfWork unitOfWork,
    IStorageService storageService)
    : IRequestHandler<UploadDocumentCommand, Guid>
{
    public async Task<Guid> Handle(UploadDocumentCommand request, CancellationToken cancellationToken)
    {
        //Here i'd call user & consumer domain services to verify the users

        var upload = request.UploadedModel;
        var documentInventory = await unitOfWork.GetDocumentInventory(cancellationToken);
        var document = new Document
        {
            Id = Guid.NewGuid(),
            Name = upload.FileName,
            UserId = upload.UserId,
            CustomerId = upload.CustomerId,
            DocumenType = upload.DocumentType
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