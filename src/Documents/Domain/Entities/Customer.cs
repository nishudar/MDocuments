using Common.Abstracts;

namespace Documents.Domain.Entities;

public class Customer : Entity
{
    public Guid AssignedUserId { get; set; }
    public string Name { get; set; }

    public void ReassignUser(Customer customer)
    {
        this.AssignedUserId = customer.AssignedUserId;
    }
}