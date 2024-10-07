using Storage.Domain;

namespace Storage.Application.Interfaces;

internal interface IFileWriter
{
    Task SaveFile(IFormFile file, FileMetadata metadata, CancellationToken cancellationToken);
}