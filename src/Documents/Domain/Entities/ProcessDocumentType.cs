namespace Documents.Domain.Entities;

public class ProcessDocumentType
{
    public Guid ProcessId { get; set; }
    public Process Process { get; set; } = null!;
    
    public Guid DocumentTypeId { get; set; }
    public DocumentType DocumentType { get; set; } = null!;
    public bool IsRequired { get; set; }
    public string? TypeName { get; set; }
    public bool MultipleAllowed { get; set; }
}