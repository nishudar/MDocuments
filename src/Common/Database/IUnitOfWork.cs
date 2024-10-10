namespace Common.Database;

public interface IUnitOfWork : IDisposable, IAsyncDisposable
{
    public Task SaveChanges(CancellationToken ct = default);
}