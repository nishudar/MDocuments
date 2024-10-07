using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Common.Abstracts;
using Documents.Domain.Enums;
using Documents.Domain.Exceptions;

namespace Documents.Domain.Entities;

public class User : Entity
{
    private string _role;
    [Required] public required string? Name { get; set; }

    [Required]
    public required string Role
    {
        get => _role;
        [MemberNotNull(nameof(_role))]
        set
        {
            if (!UserRole.Roles.Contains(value))
                throw new RoleNotExistsException(value);
            _role = value;
        }
    }

    public void SetName(string name)
    {
        Name = name;
    }
}