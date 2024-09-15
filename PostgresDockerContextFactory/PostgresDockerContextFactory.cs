using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;

namespace TestingFixtures;

/// <inheritdoc cref="IDbContextFactory{TContext}"/>
/// Uses docker and test-containers to create a real postgre-sql-database.
/// <remarks>DbContext is expected to implement a ctor like: DbContext(DbContextOptions options) </remarks>
public partial class PostgresDockerContextFactory<TCtx> : IDbContextFactory<TCtx>, IAsyncDisposable, IDisposable
    where TCtx : DbContext
{
    private readonly PostgreSqlContainer _postgreSqlContainer;
    private readonly DbContextOptions<TCtx> _options;
    private readonly Func<DbContextOptions<TCtx>, TCtx> _ctxFactory;

    /// <inheritdoc cref="IDbContextFactory{TContext}"/>
    protected PostgresDockerContextFactory(DbContextOptions<TCtx> options,
        Func<DbContextOptions<TCtx>, TCtx> ctxFactory, PostgreSqlContainer postgreSqlContainer)
    {
        _options = options;
        _ctxFactory = ctxFactory;
        _postgreSqlContainer = postgreSqlContainer;
    }

    /// <inheritdoc />
    public TCtx CreateDbContext()
    {
        return _ctxFactory(_options);
    }

    /// <inheritdoc />
    public Task<TCtx> CreateDbContextAsync(CancellationToken cancellationToken = default)
        => Task.FromResult(CreateDbContext());

    /// <inheritdoc />
    ~PostgresDockerContextFactory()
    {
        try
        {
            _postgreSqlContainer.DisposeAsync().GetAwaiter().GetResult();
        }
        catch
        {
            // ignored
        }
    }

    /// <inheritdoc />
    public async void Dispose()
    {
        await _postgreSqlContainer.DisposeAsync();
    }

    /// <inheritdoc />
    public ValueTask DisposeAsync()
    {
        return _postgreSqlContainer.DisposeAsync();
    }
}