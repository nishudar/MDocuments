using Storage.Application.Interfaces;
using Storage.Domain;

namespace Storage.Infrastructure;

public class FileReader : IFileReader
{
    private readonly string _storageDirectory;
    public FileReader(string storageDirectory)
    {
        _storageDirectory = storageDirectory ?? throw new ArgumentNullException(nameof(storageDirectory));
        if (!Directory.Exists(_storageDirectory))
            Directory.CreateDirectory(_storageDirectory);
    }

    public IFileReader.FileStreamResult? GetFile(FileMetadata fileMetadata)
    {
        if (fileMetadata.Status is not FileStatus.Completed)
            throw new IOException("File does not exist") {Data = {{"metadata", fileMetadata}}};

        var filePath = Path.Combine(_storageDirectory, fileMetadata.FilePath);

        if (!File.Exists(filePath))
            throw new IOException("File does not exist") {Data = {{"metadata", fileMetadata}}};

        var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
        var fileType = fileMetadata.Type;
        var fileName = fileMetadata.Name;

        return new IFileReader.FileStreamResult(fileStream, fileName, fileType);
    }
}