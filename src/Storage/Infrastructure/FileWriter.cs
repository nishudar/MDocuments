using Storage.Application.Interfaces;
using Storage.Domain;

namespace Storage.Infrastructure;

public class FileWriter : IFileWriter
{
    private readonly string _storageDirectory;
    public FileWriter(string storageDirectory)
    {
        _storageDirectory = storageDirectory ?? throw new ArgumentNullException(nameof(storageDirectory));
        if (!Directory.Exists(_storageDirectory))
            Directory.CreateDirectory(_storageDirectory);
    }

    public async Task SaveFile(IFormFile file, FileMetadata metadata, CancellationToken cancellationToken)
    {
        var filePath = Path.Combine(_storageDirectory, metadata.FilePath);
        await using var stream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(stream);
    }
}