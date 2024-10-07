using Storage.Application.Interfaces;
using Storage.Domain;

namespace Storage.Infrastructure;

internal class FileMetadataRepository : IFileMetadataRepository
{
    private readonly Dictionary<Guid, FileMetadata> _files = new();

    public Task<Guid> SetFileMetadata(FileMetadata fileMetadata, CancellationToken ct)
    {
        _files[fileMetadata.Id] = fileMetadata;
        return Task.FromResult(fileMetadata.Id);
    }

    public Task<FileMetadata?> GetFileMetadata(Guid fileId, CancellationToken ct)
    {
        _files.TryGetValue(fileId, out var metadata);
        return Task.FromResult(metadata);
    }

    public Task<IEnumerable<FileMetadata>> GetAllUserFileMetadata(Guid? userId, CancellationToken cancellationToken)
    {
        return Task.FromResult(userId is null
            ? _files.Values.AsEnumerable()
            : _files.Values.AsEnumerable()
                .Where(file => file.UserId == userId));
    }

    public Task<FileStatus?> GetFileStatus(Guid fileId, CancellationToken ct)
    {
        return _files.TryGetValue(fileId, out var metadata)
            ? Task.FromResult<FileStatus?>(metadata.Status)
            : Task.FromResult<FileStatus?>(null);
    }
}