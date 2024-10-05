using Storage.Domain;

namespace Storage.Application.Interfaces;

public interface IFileMetadataRepository
{
    Task<FileMetadata?> GetFileMetadata(Guid fileId, CancellationToken ct);
    Task<IEnumerable<FileMetadata>> GetAllUserFileMetadata(Guid? userId, CancellationToken cancellationToken);
    Task<FileStatus?> GetFileStatus(Guid fileId, CancellationToken ct);
    public Task<Guid> SetFileMetadata(FileMetadata fileMetadata, CancellationToken ct);
}