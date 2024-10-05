using Documents.Api.Models;
using Documents.Application.Commands;
using Documents.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Documents.Api;

public static class DocumentEndpoints
{
    private const string TagDocuments = "Documents";

    public static void MapDocumentsEndpoints(this IEndpointRouteBuilder app, TimeSpan timeout)
    {
        app.MapPost("/v1/document/files/upload", async (
                [FromForm] DocumentUploadModel upload,
                IMediator mediator
            ) =>
            {
                using var cts = new CancellationTokenSource(timeout);
                var result = await mediator.Send(new UploadDocumentCommand(upload), cts.Token);

                return Results.Ok(new IdResponse(result));
            })
            .DisableAntiforgery()
            .WithName("uploadDocument")
            .WithTags(TagDocuments)
            .Produces<IdResponse>()
            .Produces(StatusCodes.Status400BadRequest)
            .WithOpenApi(operation =>
            {
                operation.Summary = "Upload a document file";
                operation.Description = "Uploads a file and associates it with a document.";

                return operation;
            });

        app.MapGet("/v1/documents/files/download/{documentId:guid}", async (
                [FromRoute] Guid documentId,
                IMediator mediator
            ) =>
            {
                using var cts = new CancellationTokenSource(timeout);
                var file = await mediator.Send(new DownloadFileQuery(documentId), cts.Token);

                Results.File(file.FileStream, file.ContentType, file.FileName);
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