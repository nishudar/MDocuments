using Documents.Application.Interfaces;
using Documents.Domain.Aggregates;
using Documents.Domain.Entities;
using Documents.Domain.ValueTypes;

namespace Documents.Infrastructure;


/// This repository implementation is very simplified due to the limitation of the solution to in-memory operations.

public class DocumentInventoryRepository : IDocumentInventoryRepository
{
    private List<BusinessUser> Users { get; } = [];
    private List<Customer> Customers { get;  } = [];
    private List<Process> Processes { get;  } = [];
    
    private List<DocumentType> AllowedDocumentTypes { get; } =
    [
        new("RequiredDocument1", true, false),
        new("RequiredDocument2", true, false),
        new("RequiredDocument3", true, false),
        new("Optional1Multiple", false, true),
        new("Optional2Single", false, false)
    ];
    

    public Task<IDocumentsInventory> GetDocumentInventory(CancellationToken ct)
    {
        return Task.FromResult<IDocumentsInventory>(new DocumentsInventory(
            Users.ToArray(), 
            Customers.ToArray(),
            AllowedDocumentTypes.ToArray(), 
            Processes.ToArray()));
    }

    public Task AddBusinessUser(BusinessUser user, CancellationToken ct)
    {
        Users.Add(user);
        
        return Task.CompletedTask;
    }

    public Task UpdateBusinessUser(BusinessUser user, CancellationToken ct)
    {
        var existingUser = Users.Find(u => u.Id == user.Id);
        existingUser?.Set(user);
        return Task.CompletedTask;
    }

    public Task AddCustomer(Customer customer, CancellationToken ct)
    {
        if(!Customers.Exists(c => c.Id == customer.Id))
            Customers.Add(customer);
        
        return Task.CompletedTask;
    }

    public Task AssignCustomer(Customer customer, CancellationToken ct)
    {
        var existingCustomer = Customers.Find(c => c.Id == customer.Id);
        existingCustomer?.ReassignUser(customer);

        return Task.CompletedTask;
    }

    public Task AddDocument(Document document, CancellationToken ct)
    {
        var process = Processes.Find(p =>
            p.CustomerId == document.CustomerId &&
            p.BusinessUserId == document.UserId);
        process?.AddDocument(document);

        return Task.CompletedTask;
    }
    
    
    public Task UpdateProcessStatus(Process process, CancellationToken ct)
    {
        if(!Processes.Exists(p => p.Id == process.Id))
            Processes.Add(process);
        process.SetStatus(process.Status);
        
        return Task.CompletedTask;
    }
}