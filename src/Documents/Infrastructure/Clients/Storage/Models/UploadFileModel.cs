using Refit;

namespace Documents.Infrastructure.Clients.Storage.Models;

public class UploadFileModel
{
    [AliasAs("fileName")]
    public string FileName { get; set; }
    
    [AliasAs("fileType")]
    public string FileType { get; set; }
    
    [AliasAs("userId")]
    public Guid UserId { get; set; }
    
    [AliasAs("file")]
    public Stream File { get; set; }
}
