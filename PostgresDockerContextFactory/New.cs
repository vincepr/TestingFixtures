using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Testcontainers.PostgreSql;

namespace TestingFixtures;

public partial class PostgresDockerContextFactory<TCtx>
{
      /// <summary>
    /// Initializes the <see cref="IDbContextFactory{TContext}"/> by trying to find a suitable constructor via reflection.
    /// </summary>
    /// <returns>The <see cref="IDbContextFactory{TContext}"/>.</returns>
    /// <remarks>DbContext is expected to implement a ctor like: DbContext(DbContextOptions options) </remarks>
    public static async Task<PostgresDockerContextFactory<TCtx>> NewAsync()
    {
        PostgreSqlContainer container = await StartTestContainer();
        var opts = DbContextOptionsForContainer(container);
        var factory = new PostgresDockerContextFactory<TCtx>(opts, CtxFactoryViaReflection(opts), container);
        // await factory.CreateDbContext().Database.EnsureDeletedAsync();   // we do not need to call this, because a new container is created anyway
        await (await factory.CreateDbContextAsync()).Database.EnsureCreatedAsync();
        return factory;
    }

    
    /// <summary>
    /// Initializes the <see cref="IDbContextFactory{TContext}"/>. Requires the user to provide contextFactory.
    /// This way DbContext implementations with any custom constructors can be used.
    /// Example: "FileBasedContextFactor.New(opts => new MyContext(opts)"
    /// </summary>
    /// <returns>The <see cref="IDbContextFactory{TContext}"/>.</returns>
    public static async Task<PostgresDockerContextFactory<TCtx>> NewAsync(
        Func<DbContextOptions<TCtx>, TCtx> contextFactory)
    {
        PostgreSqlContainer container = await StartTestContainer();
        var opts = DbContextOptionsForContainer(container);
        var factory = new PostgresDockerContextFactory<TCtx>(opts, contextFactory, container);
        // await factory.CreateDbContext().Database.EnsureDeletedAsync();   // we do not need to call this, because a new container is created anyway
        await (await factory.CreateDbContextAsync()).Database.EnsureCreatedAsync();
        return factory;
    }

    private static DbContextOptions<TCtx> DbContextOptionsForContainer(PostgreSqlContainer container)
    {
        var opts = new DbContextOptionsBuilder<TCtx>();
        var dataSourceBuilder = new NpgsqlDataSourceBuilder(container.GetConnectionString());
        opts.UseNpgsql(dataSourceBuilder.Build());
        return opts.Options;
    }

    private static async Task<PostgreSqlContainer> StartTestContainer()
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
}