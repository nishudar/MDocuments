using Common.Abstracts;

namespace Documents.Domain.Entities;

public class Customer : Entity
{
    public required Guid AssignedUserId { get; set; }
    public required string Name { get; set; }

    public void ReassignUser(Customer customer)
    {
        this.AssignedUserId = customer.AssignedUserId;
    }
}