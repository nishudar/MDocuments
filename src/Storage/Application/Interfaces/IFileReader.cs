using Storage.Domain;

namespace Storage.Application.Interfaces;

public interface IFileReader
{
    FileStreamResult? GetFile(FileMetadata fileMetadata);
    
    public record FileStreamResult(Stream FileStream, string FileName, string ContentType);
}
