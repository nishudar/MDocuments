using Common.Abstracts;

namespace Common.IntegrationEvents.Events;

public record FileUploadIntegrationEvent : IIntegrationEvent
{
    public required Guid FileId { get; init; }
    public required  string FileName { get; init; }
    public required  string FileType { get; init; }
    public required string UserId { get; init; }
    public required DateTime UploadTime { get; init; }
    public FileStatus Status { get; set; }
}