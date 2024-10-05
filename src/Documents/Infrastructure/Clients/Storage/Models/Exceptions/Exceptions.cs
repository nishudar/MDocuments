namespace Documents.Infrastructure.Clients.Storage.Models.Exceptions;

public class FileServiceException(string message) : Exception(message);

public class StoredFileNotFoundException(string message) : FileServiceException(message);

public class FileUploadException(string message) : FileServiceException(message);