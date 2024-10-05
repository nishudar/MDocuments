using System.ComponentModel.DataAnnotations;

namespace Documents.Api.Models;

public record DocumentUploadModel
{
    [Required]
    public required string FileName { get; init; }
    [Required]
    public required Guid UserId { get; init; }
    [Required]
    public required Guid CustomerId { get; init; }
    [Required]
    public required IFormFile File { get; init; }
    [Required]
    public required string DocumentType { get; init; }
}