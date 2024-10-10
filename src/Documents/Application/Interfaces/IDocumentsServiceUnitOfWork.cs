using Common.Database;
using Documents.Domain.Entities;

namespace Documents.Application.Interfaces;

internal interface IDocumentsUnitOfWork : IUnitOfWork
{
    Task<Process[]> GetProcesses(CancellationToken ct);
    Task<DocumentType[]> GetDocumentTypes(CancellationToken ct);
    Task<User[]> GetUsers(CancellationToken ct);
    void AddUser(User user);
    Task UpdateUser(Guid userId, string name, CancellationToken ct = default);
    Task AddDocument(Document document, CancellationToken ct = default);
    Task UpdateProcessStatus(Process process, CancellationToken ct = default);
}