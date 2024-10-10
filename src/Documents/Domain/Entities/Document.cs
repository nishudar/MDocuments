using Common.Abstracts;

namespace Documents.Domain.Entities;

public class Document : Entity
{
    public required string Name { get; init; }
    public required string DocumenType { get; init; }
    public Guid CustomerId { get; init; }
    public Guid UserId { get; init; }
    public Guid? FileId { get; private set; }
    public Guid? ProcessId { get; set; }

    public void SetFileId(Guid fileId)
    {
        FileId = fileId;
    }
}