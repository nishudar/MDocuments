namespace Documents.Infrastructure.Clients.Storage.Models;

public class FileMetadataResponse
{
    public string FileName { get; set; }
    public string FileType { get; set; }
    public Guid UserId { get; set; }
    public Guid FileId { get; set; }
}