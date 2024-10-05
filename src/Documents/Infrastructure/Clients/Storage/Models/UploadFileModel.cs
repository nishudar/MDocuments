using Refit;

namespace Documents.Infrastructure.Clients.Storage.Models;

public class UploadFileModel
{
    [AliasAs("fileName")]
    public required string FileName { get; set; }
    
    [AliasAs("fileType")]
    public required string FileType { get; set; }
    
    [AliasAs("userId")]
    public required Guid UserId { get; set; }
    
    [AliasAs("file")]
    public required Stream File { get; set; }
}
