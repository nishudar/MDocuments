using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Common.Abstracts;
using Users.Domain.Enums;
using Users.Domain.Exceptions;

namespace Users.Domain.Entities;

internal class User : Entity
{
    private string _name;

    [Required] public required string Name { get; set; }
    [Required]
    public required string Role
    {
        get => _name;
        [MemberNotNull(nameof(_name))]
        set
        {
            if (!UserRole.Roles.Contains(value))
                throw new RoleNotExistsException(value);
            _name = value;
        }
    }
}