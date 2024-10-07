namespace Documents.Infrastructure.Clients.Storage.Models.Exceptions;

public class FileServiceException(string message) : Exception(message);