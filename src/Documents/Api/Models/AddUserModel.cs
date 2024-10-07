using System.ComponentModel.DataAnnotations;

namespace Documents.Api.Models;

internal record AddUserModel([Required]string Name, [Required]string Role)
{
}