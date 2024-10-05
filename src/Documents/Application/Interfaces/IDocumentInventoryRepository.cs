using Documents.Domain.Aggregates;
using Documents.Domain.Entities;

namespace Documents.Application.Interfaces;

public interface IDocumentInventoryRepository
{
    Task<IDocumentsInventory> GetDocumentInventory(CancellationToken ct);
    Task AddBusinessUser(BusinessUser user, CancellationToken ct);
    Task UpdateBusinessUser(BusinessUser user, CancellationToken ct);
    Task AddCustomer(Customer customer, CancellationToken ct);
    Task AssignCustomer(Customer customer, CancellationToken ct);
    Task AddDocument(Document document, CancellationToken ct);
    Task UpdateProcessStatus(Process process, CancellationToken ct);
    
}