using Common.IntegrationEvents.Events;
using MediatR;
using Storage.Application.Interfaces;
using Storage.Domain;

namespace Storage.Application.Commands;

public record UploadFileCommand(string FileName, string FileType, Guid UserId, IFormFile File) : IRequest<Guid>;

public class UploadFileHandler(IFileMetadataRepository fileMetadata, IFileWriter fileWriter, IMediator mediator)
    : IRequestHandler<UploadFileCommand, Guid>
{
    public async Task<Guid> Handle(UploadFileCommand request, CancellationToken cancellationToken)
    {
        var metadata = FileMetadata.CreateNew(request.FileName, request.FileType, request.UserId);
        var fileUploadEvent = new FileUploadIntegrationEvent
        {
            FileId = metadata.Id,
            FileName = metadata.Name,
            UserId = metadata.UserId.ToString(),
            FileType = metadata.Type,
            UploadTime = metadata.Created,
            Status = FileStatus.Uploading.ToString()
        };
        try
        {
            await fileMetadata.SetFileMetadata(metadata with {Status = FileStatus.Uploading}, cancellationToken);
            await fileWriter.SaveFile(request.File, metadata, cancellationToken);
            await fileMetadata.SetFileMetadata(metadata with {Status = FileStatus.Completed}, cancellationToken);

            await mediator.Publish(fileUploadEvent with {Status = FileStatus.Completed.ToString()}, cancellationToken);
        }
        catch (Exception)
        {
            await fileMetadata.SetFileMetadata(metadata with {Status = FileStatus.Failed}, cancellationToken);
            await mediator.Publish(fileUploadEvent with {Status = FileStatus.Failed.ToString()}, cancellationToken);
        }

        return metadata.Id;
    }
}