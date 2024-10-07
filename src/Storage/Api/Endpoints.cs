using MediatR;
using Microsoft.AspNetCore.Mvc;
using Storage.Api.Models;
using Storage.Application.Commands;
using Storage.Application.Queries;
using Storage.Domain;

namespace Storage.Api;

internal static class FileEndpoints
{
    public static void MapFileEndpoints(this IEndpointRouteBuilder app, TimeSpan operationTimeout)
    {
        const string FileEndpointName = "File";
        
        app.MapPost("/v1/file/upload", async (
                HttpContext context,
                IMediator mediator,
                [FromForm] UploadModel model) =>
            {
                using CancellationTokenSource cts = new(operationTimeout);
                var fileId =
                    await mediator.Send(new UploadFileCommand(model.FileName, model.FileType, model.UserId, model.File),
                        cts.Token);
                return Results.Ok(new {FileId = fileId});
            })
            .DisableAntiforgery()
            .WithMetadata(new IgnoreAntiforgeryTokenAttribute())
            .WithName("uploadFile")
            .WithTags(FileEndpointName)
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .WithOpenApi(operation =>
            {
                operation.Summary = "Upload a file";
                operation.Description = "Uploads a file to the server.";
                return operation;
            });

        app.MapGet("/v1/file/{id:guid}", async (IMediator mediator, Guid id) =>
            {
                using CancellationTokenSource cts = new(operationTimeout);
                var fileMetadata = await mediator.Send(new GetFileStreamQuery(id), cts.Token);
                return fileMetadata is not null
                    ? Results.File(fileMetadata.FileStream, fileMetadata.ContentType,
                        fileMetadata.FileName)
                    : Results.NotFound();
            })
            .WithName("downloadFile")
            .WithTags(FileEndpointName)
            .Produces<FileContentResult>()
            .Produces(StatusCodes.Status404NotFound)
            .WithOpenApi(operation =>
            {
                operation.Summary = "Download a file";
                operation.Description = "Retrieves a file with the specified ID.";
                var idParam = operation.Parameters.First(p => p.Name == "id");
                idParam.Description = "The unique identifier of the file";
                return operation;
            });

        app.MapGet("/v1/file/metadata", async (IMediator mediator, Guid? userId) =>
            {
                using CancellationTokenSource cts = new(operationTimeout);
                var files = await mediator.Send(new GetFilesMetadataQuery(userId), cts.Token);

                return Results.Ok(files);
            })
            .WithName("getMetadata")
            .WithTags(FileEndpointName)
            .Produces<IEnumerable<FileMetadata>>()
            .WithOpenApi(operation =>
            {
                operation.Summary = "Retrieve files metadata";
                operation.Description = "Gets metadata for all files, optionally filtered by user ID.";
                var userIdParam = operation.Parameters.FirstOrDefault(p => p.Name == "userId");
                if (userIdParam != null) userIdParam.Description = "Optional user ID to filter files";
                return operation;
            });


        app.MapGet("/v1/file/track/{trackingId:guid}", async (IMediator mediator, Guid trackingId) =>
            {
                using CancellationTokenSource cts = new(operationTimeout);
                var fileStatus = await mediator.Send(new GetFileStatusQuery(trackingId), cts.Token);

                return fileStatus is not null
                    ? Results.Ok(new {Status = fileStatus.ToString()})
                    : Results.NotFound();
            })
            .WithName("trackFile")
            .WithTags("Files")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .WithOpenApi(operation =>
            {
                operation.Summary = "Track file processing status";
                operation.Description = "Retrieves the processing status of a file using the tracking ID.";
                var trackingIdParam = operation.Parameters.First(p => p.Name == "trackingId");
                trackingIdParam.Description = "The tracking ID of the file";
                return operation;
            });
    }
}