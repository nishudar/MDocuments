using Documents.Domain.Aggregates;
using Documents.Domain.Entities;

namespace Documents.Application.Interfaces;

internal interface IDocumentInventoryRepository
{
    Task<IDocumentsInventory> GetDocumentInventory(CancellationToken ct);
    Task AddUser(User user, CancellationToken ct);
    Task UpdateBusinessUser(Guid userId, string name, CancellationToken ct);
    Task AddDocument(Document document, CancellationToken ct);
    Task UpdateProcessStatus(Process process, CancellationToken ct);
}