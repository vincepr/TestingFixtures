using Microsoft.EntityFrameworkCore;

namespace TestingFixtures;

/// <inheritdoc cref="IDbContextFactory{TContext}"/>
/// Uses sqlite to create a real file-based-database.
/// <remarks>DbContext is expected to implement a ctor like: DbContext(DbContextOptions options) </remarks>
public partial class FileBasedContextFactory<TCtx> : IDbContextFactory<TCtx>, IAsyncDisposable, IDisposable
    where TCtx : DbContext
{
    private readonly DbContextOptions<TCtx> _options;
    private readonly Func<DbContextOptions<TCtx>, TCtx> _ctxFactory;
    private readonly string _filePath;

    /// <inheritdoc cref="IDbContextFactory{TContext}"/>
    protected FileBasedContextFactory(DbContextOptions<TCtx> options, string filePath,
        Func<DbContextOptions<TCtx>, TCtx> ctxFactory)
    {
        _options = options;
        _filePath = filePath;
        _ctxFactory = ctxFactory;
    }
    
    /// <summary>
    /// Implicit conversion to the context for convenience.
    /// </summary>
    /// <param name="self">The context-factory itself.</param>
    /// <returns>A single context created from that context-factory.</returns>
    public static implicit operator TCtx(FileBasedContextFactory<TCtx> self) => self.CreateDbContext(); 

    /// <inheritdoc />
    public TCtx CreateDbContext()
    {
        return _ctxFactory(_options);
    }

    /// <inheritdoc />
    public Task<TCtx> CreateDbContextAsync(CancellationToken cancellationToken = default)
        => Task.FromResult(CreateDbContext());

    /// <inheritdoc />
    ~FileBasedContextFactory()
    {
        try
        {
            Cleanup();
        }
        catch
        {
            // ignored
        }
    }

    /// <inheritdoc />
    public ValueTask DisposeAsync()
    {
        try
        {
            Cleanup();
        }
        catch (Exception ex)
        {
            return ValueTask.FromException(ex);
        }

        return ValueTask.CompletedTask;

    }

    /// <inheritdoc />
    public void Dispose()
    {
        try
        {
            Cleanup();
        }
        catch
        {
            // ignored
        }
    }
    
    private void Cleanup() => File.Delete(_filePath);
}