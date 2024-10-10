namespace Common.Database;

public abstract class UnitOfWork : IUnitOfWork
{
    public abstract Task SaveChanges(CancellationToken ct = default);
    
    protected abstract void Dispose(bool disposing);
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    
    public abstract ValueTask DisposeAsync();
}