using Documents.Infrastructure.Clients.Storage.Models;

namespace Documents.Application.Interfaces;

internal interface IStorageService
{
    Task<FileDownloadResponse?> DownloadFileAsync(Guid fileId, CancellationToken ct = default);

    Task<UploadFileResponse> UploadFile(Stream fileStream, string fileName, string fileType, Guid userId,
        CancellationToken ct = default);

    Task<List<FileMetadataResponse>> GetUserFilesMetadata(Guid? userId = null, CancellationToken ct = default);
    Task<FileStatusResponse> GetFileStatus(Guid trackingId, CancellationToken ct = default);
}