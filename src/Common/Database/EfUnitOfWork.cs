using System.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Common.Database;

public class EfUnitOfWork<TContext> : UnitOfWork where TContext : DbContext
{
    private readonly TContext _ctx;
    private readonly IDbContextTransaction? _transaction = null;

    protected EfUnitOfWork(TContext ctx)
    {
        _ctx = ctx;
        if (!IsInMemoryDatabase()) 
            _transaction = _ctx.Database.BeginTransaction(IsolationLevel.Serializable);
    }

    public override async Task SaveChanges(CancellationToken ct = default)
    {
        await _ctx.SaveChangesAsync(ct);
        if (!IsInMemoryDatabase() && _transaction is not null) 
            await _transaction.CommitAsync(ct);
    }

    private bool IsInMemoryDatabase()
    {
        return _ctx.Database.IsInMemory();
    }

    protected override void Dispose(bool disposing)
    {
        if (!disposing) 
            return;
        
        _transaction?.Dispose();
        _ctx.Dispose();
    }

    public override async ValueTask DisposeAsync()
    {
        if(_transaction is not null)
            await _transaction.RollbackAsync();
        
        await _ctx.DisposeAsync();
    }
}
