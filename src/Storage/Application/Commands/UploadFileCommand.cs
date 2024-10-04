using Common.Abstracts;
using Common.IntegrationEvents;
using Common.IntegrationEvents.Events;
using MediatR;
using Storage.Application.Interfaces;
using Storage.Domain;

namespace Storage.Application.Commands;

public record UploadFileCommand(string FileName, string FileType, Guid UserId, IFormFile File) : IRequest<Guid>;

public class UploadFileHandler(IFileMetadataRepository fileMetadata, IFileWriter fileWriter, IIntegrationEventProducer integrationEventProducer)
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
            Status = FileStatus.Uploading
        };
        try
        {
            await fileMetadata.SetFileMetadata(metadata with {Status = FileStatus.Uploading}, cancellationToken);
            await fileWriter.SaveFile(request.File, metadata, cancellationToken);
            await fileMetadata.SetFileMetadata(metadata with {Status = FileStatus.Completed}, cancellationToken);
            await integrationEventProducer.SendEvent(IntegrationTopics.FileUploadsTopic, metadata.Id.ToString(),
                fileUploadEvent with {Status = FileStatus.Completed}, cancellationToken);
        }
        catch (Exception)
        {
            await integrationEventProducer.SendEvent(IntegrationTopics.FileUploadsTopic, metadata.Id.ToString(), fileUploadEvent, cancellationToken);
            await fileMetadata.SetFileMetadata(metadata with{Status = FileStatus.Failed}, cancellationToken);
            await integrationEventProducer.SendEvent(IntegrationTopics.FileUploadsTopic, metadata.Id.ToString(), fileUploadEvent with{Status = FileStatus.Failed}, cancellationToken);
        }
        
        return metadata.Id;
    }
}