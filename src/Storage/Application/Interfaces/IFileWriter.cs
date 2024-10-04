using Storage.Domain;

namespace Storage.Application.Interfaces;

public interface IFileWriter
{
    Task SaveFile(IFormFile file, FileMetadata metadata, CancellationToken cancellationToken);
}