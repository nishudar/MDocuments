using Storage.Domain;

namespace Storage.Application.Interfaces;

internal interface IFileReader
{
    FileStreamResult? GetFile(FileMetadata fileMetadata);

    public record FileStreamResult(Stream FileStream, string FileName, string ContentType);
}