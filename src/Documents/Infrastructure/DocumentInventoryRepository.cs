using System.Collections.Concurrent;
using Documents.Application.Interfaces;
using Documents.Domain.Aggregates;
using Documents.Domain.Entities;
using Documents.Domain.ValueTypes;
using Force.DeepCloner;

namespace Documents.Infrastructure;


/// This repository implementation is very simplified due to the limitation of the solution to in-memory operations.

public class DocumentInventoryRepository : IDocumentInventoryRepository
{
    private ConcurrentBag<BusinessUser> Users { get; } = [];
    private ConcurrentBag<Customer> Customers { get;  } = [];
    private ConcurrentBag<Process> Processes { get;  } = [];
    
    private ConcurrentBag<DocumentType> AllowedDocumentTypes { get; } =
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
            Users.ToArray().DeepClone(), 
            Customers.ToArray().DeepClone(),
            AllowedDocumentTypes.ToArray().DeepClone(), 
            Processes.ToArray().DeepClone()));
    }

    public Task AddBusinessUser(BusinessUser user, CancellationToken ct)
    {
        Users.Add(user);
        
        return Task.CompletedTask;
    }

    public Task UpdateBusinessUser(BusinessUser user, CancellationToken ct)
    {
        var existingUser = Users.FirstOrDefault(u => u.Id == user.Id);
        existingUser?.Set(user);
        return Task.CompletedTask;
    }

    public Task AddCustomer(Customer customer, CancellationToken ct)
    {
        if(Customers.All(c => c.Id != customer.Id))
            Customers.Add(customer);
        
        return Task.CompletedTask;
    }

    public Task AssignCustomer(Customer customer, CancellationToken ct)
    {
        var existingCustomer = Customers.FirstOrDefault(c => c.Id == customer.Id);
        existingCustomer?.ReassignUser(customer);

        return Task.CompletedTask;
    }

    public Task AddDocument(Document document, CancellationToken ct)
    {
        var process = Processes.FirstOrDefault(p =>
            p.CustomerId == document.CustomerId &&
            p.BusinessUserId == document.UserId);
        process?.AddDocument(document);

        return Task.CompletedTask;
    }
    
    
    public Task UpdateProcessStatus(Process process, CancellationToken ct)
    {
        if(Processes.All(p => p.Id != process.Id))
            Processes.Add(process);
        process.SetStatus(process.Status);
        
        return Task.CompletedTask;
    }
}