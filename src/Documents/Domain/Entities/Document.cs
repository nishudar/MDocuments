using Common.Abstracts;

namespace Documents.Domain.Entities;

public class Document : Entity
{
    public string Name { get; init; }
    public string DocumenType { get; init; }
    public Guid CustomerId { get; init; }
    public Guid UserId { get; init; }
    public Guid? FileId { get; private set; }

    public void SetFileId(Guid fileId)
    {
        this.FileId = fileId;
    }
}

