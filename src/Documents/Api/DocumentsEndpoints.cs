using Common.DomainEvents;
using Documents.Api.Models;
using Documents.Application.Interfaces;
using Documents.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Documents.Api;

public static class DocumentEndpoints
{
    private const string TagDocuments = "Documents";
    public static void MapDocumentsEndpoints(this IEndpointRouteBuilder app, TimeSpan timeout)
    {
        app.MapPost("/v1/document/files/upload", async (
                [FromForm] DocumentUploadModel upload,
                IDocumentInventoryRepository repository,
                IDomainEventDispatcher dispatcher,
                IStorageService storageService
            ) =>
            {
                using var cts = new CancellationTokenSource(timeout);
                var documentInventory = await repository.GetDocumentInventory(cts.Token);
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
                    cts.Token
                );
                document.SetFileId(uploadResponse.FileId);
                documentInventory.AddDocument(document);
                await dispatcher.DispatchEvents(documentInventory.BusinessEvents, cts.Token);
            })
            .DisableAntiforgery()
            .WithName("uploadDocument")
            .WithTags(TagDocuments)
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .WithOpenApi(operation =>
            {
                operation.Summary = "Upload a document file";
                operation.Description = "Uploads a file and associates it with a document.";

                return operation;
            });

        app.MapGet("/v1/documents/files/download/{documentId:guid}", async (
                [FromRoute] Guid documentId,
                IDocumentInventoryRepository repository,
                IStorageService storageService
            ) =>
            {
                using var cts = new CancellationTokenSource(timeout);
                var documentInventory = await repository.GetDocumentInventory(cts.Token);
                var document = documentInventory.GetDocument(documentId);
                if (document is null && document is {FileId: null})
                    return Results.NotFound();
                var file = await storageService.DownloadFileAsync(document!.FileId!.Value, cts.Token);
                return file switch
                {
                    null => Results.NotFound(),
                    _ => Results.File(file.FileStream, file.ContentType, file.FileName)
                };
            })
            .DisableAntiforgery()
            .WithName("downloadDocument")
            .WithTags(TagDocuments)
            .Produces<FileContentResult>()
            .Produces(StatusCodes.Status404NotFound)
            .WithOpenApi(operation =>
            {
                operation.Summary = "Download a document file";
                operation.Description = "Downloads the file associated with a document";
                var documentIdParam = operation.Parameters.First(p => p.Name == "documentId");
                documentIdParam.Description = "The unique identifier of the document";
                return operation;
            });
    }
}
