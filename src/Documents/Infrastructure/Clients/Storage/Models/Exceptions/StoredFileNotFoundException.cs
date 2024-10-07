namespace Documents.Infrastructure.Clients.Storage.Models.Exceptions;

public class StoredFileNotFoundException(string message) : FileServiceException(message);