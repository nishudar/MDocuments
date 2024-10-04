using System.Text.RegularExpressions;
using Common.Abstracts;

namespace Storage.Domain;

public partial record FileMetadata
{
    public Guid Id { get; private init; }
    public required string Name { get; init; }
    public required string Type { get; init; }
    public required Guid UserId { get; init; }
    public FileStatus Status { get; init; }
    public required string FilePath { get; init; }
    public required DateTime Created { get; set; }

    public static FileMetadata CreateNew(string fileName, string fileType, Guid userId)
    {
        var id = Guid.NewGuid();
        var filePath = $"{id}_{SpecialCharactersRegex().Replace(fileName, "")}";
        var metadata = new FileMetadata
        {
            Id = id,
            Name = fileName,
            Type = fileType,
            UserId = userId,
            Status = FileStatus.Uploading,
            FilePath = filePath,
            Created = DateTime.UtcNow
        };
        
        return metadata;
    }

    [GeneratedRegex(@"[^a-zA-Z0-9\-_\.]")]
    private static partial Regex SpecialCharactersRegex();
}