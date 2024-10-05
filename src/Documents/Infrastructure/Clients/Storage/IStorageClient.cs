using Documents.Infrastructure.Clients.Storage.Models;
using Refit;

namespace Documents.Infrastructure.Clients.Storage;

public interface IStorageClient
{
    [Multipart]
    [Post("/v1/file/upload")]
    Task<ApiResponse<UploadFileResponse>> UploadFile([AliasAs("model")] MultipartFormDataContent content);

    [Get("/v1/file/{id}")]
    Task<HttpResponseMessage> DownloadFile(Guid id, CancellationToken ct);

    [Get("/v1/file/metadata")]
    Task<ApiResponse<List<FileMetadataResponse>>> GetUserFilesMetadata([Query] Guid? userId = null,
        CancellationToken ct = default);

    [Get("/v1/file/track/{trackingId}")]
    Task<ApiResponse<FileStatusResponse>> GetFileStatus(Guid trackingId, CancellationToken ct);
}