using System.ComponentModel.DataAnnotations;
using Common.Abstracts;

namespace Documents.Domain.Entities;

public class BusinessUser : Entity
{
    [Required] public required string? Name { get; set; }

    public void Set(BusinessUser user)
    {
        Name = user.Name;
    }
}