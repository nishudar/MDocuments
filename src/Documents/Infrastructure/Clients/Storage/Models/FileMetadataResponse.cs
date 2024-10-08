namespace Documents.Infrastructure.Clients.Storage.Models;

internal class FileMetadataResponse
{
    public required string FileName { get; set; }
    public required string FileType { get; set; }
    public required Guid UserId { get; set; }
    public required Guid FileId { get; set; }
}