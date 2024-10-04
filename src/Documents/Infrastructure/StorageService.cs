using Documents.Application.Interfaces;
using Documents.Infrastructure.Clients.Storage;
using Documents.Infrastructure.Clients.Storage.Models;
using Documents.Infrastructure.Clients.Storage.Models.Exceptions;

namespace Documents.Infrastructure;

public class StorageService(IStorageClient fileServiceClient) : IStorageService
{
    public async Task<FileDownloadResponse?> DownloadFileAsync(Guid fileId, CancellationToken ct)
    {
        var response = await fileServiceClient.DownloadFile(fileId, ct);
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            return null;
        
        if (response.IsSuccessStatusCode)
        {
            var fileStream = await response.Content.ReadAsStreamAsync(ct);
            var contentType = response.Content.Headers.ContentType?.ToString() ?? "application/octet-stream";
            var fileName = response.Content.Headers.ContentDisposition?.FileName?.Trim('"') ?? "downloadedFile";
            return new FileDownloadResponse
            {
                FileStream = fileStream,
                ContentType = contentType,
                FileName = fileName
            };
        }

        throw new FileServiceException($"Failed to download file. Status code: {response.StatusCode}");
    }

    public async Task<UploadFileResponse> UploadFile(Stream fileStream, string fileName, string fileType, Guid userId, CancellationToken ct)
    {
        var fileContent = new StreamContent(fileStream);
        var multipartContent = new MultipartFormDataContent
        {
            { fileContent, "file", fileName },
            { new StringContent(fileType), "fileType" },
            { new StringContent(userId.ToString()), "userId" },
            { new StringContent(fileName), "fileName" }
            
        };
        var response = await fileServiceClient.UploadFile(multipartContent);
        
        return response.Content!;
    }
    
    public async Task<List<FileMetadataResponse>> GetUserFilesMetadata(Guid? userId = null,  CancellationToken ct = default)
    {
        var response = await fileServiceClient.GetUserFilesMetadata(userId, ct);
        if (response.IsSuccessStatusCode)
            return response.Content;

        throw new FileServiceException("Failed to retrieve file metadata.");
    }
    
    public async Task<FileStatusResponse> GetFileStatus(Guid trackingId,  CancellationToken ct)
    {
        var response = await fileServiceClient.GetFileStatus(trackingId, ct);
        if (response.IsSuccessStatusCode)
            return response.Content;

        throw new FileServiceException("Failed to retrieve file status.");
    }
}