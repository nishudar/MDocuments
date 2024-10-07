namespace Documents.Infrastructure.Clients.Storage.Models;

internal class FileDownloadResponse
{
    public required Stream FileStream { get; init; }
    public required string ContentType { get; init; }
    public required string FileName { get; init; }
}