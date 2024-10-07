namespace Documents.Infrastructure.Clients.Storage.Models.Exceptions;

public class FileUploadException(string message) : FileServiceException(message);