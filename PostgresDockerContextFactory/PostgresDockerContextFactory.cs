using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Testcontainers.PostgreSql;

namespace TestingFixtures;

/// <inheritdoc cref="IDbContextFactory{TContext}"/>
/// <remarks>DbContext is expected to implement a ctor like: DbContext(DbContextOptions options) </remarks>
public class PostgresDockerContextFactory<TCtx> : IDbContextFactory<TCtx>, IAsyncDisposable, IDisposable
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

    /// <summary>
    /// Initializes the <see cref="IDbContextFactory{TContext}"/> by trying to find a suitable constructor via reflection.
    /// </summary>
    /// <returns>The <see cref="IDbContextFactory{TContext}"/>.</returns>
    /// <remarks>DbContext is expected to implement a ctor like: DbContext(DbContextOptions options) </remarks>
    public static async Task<PostgresDockerContextFactory<TCtx>> New()
    {
        PostgreSqlContainer container = await CreateNewTestContainer();
        var opts = new DbContextOptionsBuilder<TCtx>();
        var dataSourceBuilder = new NpgsqlDataSourceBuilder(container.GetConnectionString());
        opts.UseNpgsql(dataSourceBuilder.Build());
        var factory = new PostgresDockerContextFactory<TCtx>(opts.Options, CtxFactoryViaReflection(opts.Options), container);
        // await factory.CreateDbContext().Database.EnsureDeletedAsync();   // we do not need to call this, because a new container is created anyway
        await (await factory.CreateDbContextAsync()).Database.EnsureCreatedAsync();
        return factory;
    }

    private static async Task<PostgreSqlContainer> CreateNewTestContainer()
    {
        var container = new PostgreSqlBuilder().Build();
        await container.StartAsync();
        return container;
    }

    private static Func<DbContextOptions<TCtx>, TCtx> CtxFactoryViaReflection(DbContextOptions<TCtx> options)
    {
        // run it once to ensure it crashes on creation:
        Type type = typeof(TCtx);
        ConstructorInfo? ctor = type.GetConstructor(new[] { typeof(DbContextOptions<TCtx>) });
        object? instance = ctor?.Invoke(new object[] { options });
        _ = instance as TCtx ?? throw new InvalidOperationException(
            $"Reflection failed. Could not locate ctor. Just provide the ContextFactory manually. ex:  '{nameof(PostgresDockerContextFactory<TCtx>)}<MyCtx>.New(opt => MyCtx(opt))'");
        return (opts) => (TCtx)ctor?.Invoke(new object[] { opts })!;
    }

    /// <summary>
    /// Initializes the <see cref="IDbContextFactory{TContext}"/>. Requires the user to provide contextFactory.
    /// This way DbContext implementations with any custom constructors can be used.
    /// Example: "FileBasedContextFactor.New(opts => new MyContext(opts)"
    /// </summary>
    /// <returns>The <see cref="IDbContextFactory{TContext}"/>.</returns>
    public static async Task<PostgresDockerContextFactory<TCtx>> New(
        Func<DbContextOptions<TCtx>, TCtx> contextFactory)
    {
        PostgreSqlContainer container = await CreateNewTestContainer();
        var opts = new DbContextOptionsBuilder<TCtx>();
        var dataSourceBuilder = new NpgsqlDataSourceBuilder(container.GetConnectionString());
        opts.UseNpgsql(dataSourceBuilder.Build());
        var factory = new PostgresDockerContextFactory<TCtx>(opts.Options, contextFactory, container);
        // await factory.CreateDbContext().Database.EnsureDeletedAsync();   // we do not need to call this, because a new container is created anyway
        await (await factory.CreateDbContextAsync()).Database.EnsureCreatedAsync();
        return factory;
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