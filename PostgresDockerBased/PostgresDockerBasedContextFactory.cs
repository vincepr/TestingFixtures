using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Testcontainers.PostgreSql;

namespace ClassLibrary1;

public class PostgresDockerBasedContextFactory<TCtx> : IDbContextFactory<TCtx>, IAsyncDisposable, IDisposable
    where TCtx : DbContext
{
    private readonly PostgreSqlContainer _postgreSqlContainer;
    private readonly DbContextOptions<TCtx> _options;
    private readonly Func<DbContextOptions<TCtx>, TCtx> _ctxFactory;

    protected PostgresDockerBasedContextFactory(DbContextOptions<TCtx> options,
        Func<DbContextOptions<TCtx>, TCtx> ctxFactory, PostgreSqlContainer postgreSqlContainer)
    {
        _options = options;
        _ctxFactory = ctxFactory;
        _postgreSqlContainer = postgreSqlContainer;
    }

    // one way is to find constructor via reflection:
    public static async Task<PostgresDockerBasedContextFactory<TCtx>> New()
    {
        PostgreSqlContainer container = await CreateNewTestContainer();
        var opts = new DbContextOptionsBuilder<TCtx>();
        var dataSourceBuilder = new NpgsqlDataSourceBuilder(container.GetConnectionString());
        opts.UseNpgsql(dataSourceBuilder.Build());
        var factory = new PostgresDockerBasedContextFactory<TCtx>(opts.Options, CtxFactoryViaReflection(opts.Options), container);
        // await factory.CreateDbContext().Database.EnsureDeletedAsync();   // we do not need to call this, because a new container is created anyway
        await (await factory.CreateDbContextAsync()).Database.EnsureCreatedAsync();
        return factory;
    }

    private static async Task<PostgreSqlContainer> CreateNewTestContainer()
    {
        var container = new PostgreSqlBuilder()
            .Build();
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
            "Reflection failed. Could not locate ctor. Just provide the ContextFactory manually. ex: 'PostgresDockerBasedContextFactory<MyCtx>.New(opt => MyCtx(opt))'");
        return (opts) => (TCtx)ctor?.Invoke(new object[] { opts })!;
    }

    // alternative we provide a way to manually provide it via a factory:
    public static async Task<PostgresDockerBasedContextFactory<TCtx>> New(
        Func<DbContextOptions<TCtx>, TCtx> contextFactory)
    {
        PostgreSqlContainer container = await CreateNewTestContainer();
        var opts = new DbContextOptionsBuilder<TCtx>();
        var dataSourceBuilder = new NpgsqlDataSourceBuilder(container.GetConnectionString());
        opts.UseNpgsql(dataSourceBuilder.Build());
        var factory = new PostgresDockerBasedContextFactory<TCtx>(opts.Options, contextFactory, container);
        // await factory.CreateDbContext().Database.EnsureDeletedAsync();   // we do not need to call this, because a new container is created anyway
        await (await factory.CreateDbContextAsync()).Database.EnsureCreatedAsync();
        return factory;
    }

    public TCtx CreateDbContext()
    {
        return _ctxFactory(_options);
    }

    public Task<TCtx> CreateDbContextAsync(CancellationToken cancellationToken = default)
        => Task.FromResult(CreateDbContext());

    ~PostgresDockerBasedContextFactory()
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

    public async void Dispose()
    {
        await _postgreSqlContainer.DisposeAsync();
    }

    public ValueTask DisposeAsync()
    {
        return _postgreSqlContainer.DisposeAsync();
    }
}