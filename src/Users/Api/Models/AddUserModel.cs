using System.ComponentModel.DataAnnotations;

namespace Users.Api.Models;

internal record AddUserModel([Required] string Name, [Required] string Role);