using System.ComponentModel.DataAnnotations;

namespace Storage.Api.Models;

internal record UploadModel
{
    [Required]
    public required string FileName { get; init; }

    [Required]
    public required string FileType { get; init; }

    [Required]
    public required Guid UserId { get; init; }

    [Required]
    public required IFormFile File { get; init; }
}