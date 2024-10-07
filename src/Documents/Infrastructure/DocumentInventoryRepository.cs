using System.Collections.Concurrent;
using Documents.Application.Interfaces;
using Documents.Domain.Aggregates;
using Documents.Domain.Entities;
using Documents.Domain.Exceptions;
using Documents.Domain.ValueObjects;
using Force.DeepCloner;

namespace Documents.Infrastructure;

/// This repository implementation is very simplified due to the limitation of the solution to in-memory operations.
internal class DocumentInventoryRepository : IDocumentInventoryRepository
{
    private ConcurrentBag<User> Users { get; } = [];
    private ConcurrentBag<Process> Processes { get; } = [];

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
            AllowedDocumentTypes.ToArray().DeepClone(),
            Processes.ToArray().DeepClone()));
    }

    public Task AddUser(User user, CancellationToken ct)
    {
        Users.Add(user);

        return Task.CompletedTask;
    }

    public Task UpdateBusinessUser(Guid userId, string name, CancellationToken ct)
    {
        var existingUser = Users.FirstOrDefault(u => u.Id == userId);
        existingUser?.SetName(name);
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
        if (Processes.All(p => p.Id != process.Id))
            Processes.Add(process);
        var existingProcess = Processes.FirstOrDefault(p => p.Id == process.Id);
        if (existingProcess is null)
            throw new ProcessNotFoundException();
        
        existingProcess.SetStatus(process.Status);

        return Task.CompletedTask;
    }
}