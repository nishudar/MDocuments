using Common.Database;
using Documents.Application.Interfaces;
using Documents.Domain.Entities;
using Documents.Domain.Exceptions;
using Force.DeepCloner;
using Microsoft.EntityFrameworkCore;

namespace Documents.Infrastructure.Database;

internal sealed class DocumentsUnitOfWork(DocumentsServiceContext ctx)
    : EfUnitOfWork<DocumentsServiceContext>(ctx), IDocumentsUnitOfWork
{
    private readonly DocumentsServiceContext _ctx = ctx;

    public async Task<Process[]> GetProcesses(CancellationToken ct)
        => await _ctx.Processes.AsNoTracking().ToArrayAsync(ct);

    public async Task<DocumentType[]> GetDocumentTypes(CancellationToken ct)
        => await _ctx.AllowedDocumentTypes.AsNoTracking().ToArrayAsync(ct);

    public async Task<User[]> GetUsers(CancellationToken ct)
        => await _ctx.Users.AsNoTracking().ToArrayAsync(ct);

    public void AddUser(User user)
    {
        ArgumentNullException.ThrowIfNull(user);
        _ctx.Users.Add(user);
    }

    public async Task UpdateUser(Guid userId, string name, CancellationToken ct = default)
    {
        var existingUser = await _ctx.Users.FirstOrDefaultAsync(u => u.Id == userId, ct);
        if (existingUser is null)
            throw new UserNotExistsException(userId);

        existingUser.SetName(name);
    }

    public async Task AddDocument(Document document, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(document);

        var process = await _ctx.Processes.FirstOrDefaultAsync(p =>
            p.CustomerId == document.CustomerId &&
            p.OperatorUserId == document.UserId, ct);

        if (process is null)
            throw new ProcessNotFoundException(document.CustomerId, document.UserId);

        process.AddDocument(document);
    }

    public async Task UpdateProcessStatus(Process process, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(process);

        var existingProcess = await _ctx.Processes.FirstOrDefaultAsync(p => p.Id == process.Id, ct);
        if (existingProcess is null)
            _ctx.Processes.Add(process.DeepClone()); 
        else
            existingProcess.Status = process.Status;
    }
    
}