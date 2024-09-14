using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace TestingFixtures;

/// <inheritdoc cref="IDbContextFactory{TContext}"/>
/// <remarks>DbContext is expected to implement a ctor like: DbContext(DbContextOptions options) </remarks>
public class FileBasedContextFactory<TCtx> : IDbContextFactory<TCtx>, IAsyncDisposable, IDisposable
    where TCtx : DbContext
{
    private readonly DbContextOptions<TCtx> _options;
    private readonly Func<DbContextOptions<TCtx>, TCtx> _ctxFactory;
    private readonly string _filePath;

    /// <inheritdoc cref="IDbContextFactory{TContext}"/>
    protected FileBasedContextFactory(DbContextOptions<TCtx> options, string filePath, Func<DbContextOptions<TCtx>, TCtx> ctxFactory)
    {
        _options = options;
        _filePath = filePath;
        _ctxFactory = ctxFactory;
    }
    
    /// <summary>
    /// Initializes the <see cref="IDbContextFactory{TContext}"/> by trying to find a suitable constructor via reflection.
    /// </summary>
    /// <returns>The <see cref="IDbContextFactory{TContext}"/>.</returns>
    /// <remarks>DbContext is expected to implement a ctor like: DbContext(DbContextOptions options) </remarks>
    public static async Task<FileBasedContextFactory<TCtx>> New()
    {
        var opts = new DbContextOptionsBuilder<TCtx>();
        var filePath = Path.GetTempFileName();
        opts.UseSqlite($"Data Source={filePath}.sqlite");
        var factory =  new FileBasedContextFactory<TCtx>(opts.Options, filePath, CtxFactoryViaReflection(opts.Options));
        await using var ctx = await factory.CreateDbContextAsync();
        await ctx.Database.EnsureDeletedAsync();
        await ctx.Database.EnsureCreatedAsync();
        return factory;
    }

    private static Func<DbContextOptions<TCtx>, TCtx> CtxFactoryViaReflection(DbContextOptions<TCtx> options)
    {
        // run it once to ensure it crashes on creation:
        Type type = typeof(TCtx);
        ConstructorInfo? ctor = type.GetConstructor(new[] { typeof(DbContextOptions<TCtx>) });
        object? instance = ctor?.Invoke(new object[] { options });
        _ = instance as TCtx ?? throw new InvalidOperationException($"Reflection failed. Could not locate ctor. Just provide the ContextFactory manually. ex: '{nameof(FileBasedContextFactory<TCtx>)}<MyCtx>.New(opt => MyCtx(opt))'");
        return (opts) => (TCtx)ctor?.Invoke(new object[] { opts })!;
    }
    
    /// <summary>
    /// Initializes the <see cref="IDbContextFactory{TContext}"/>. Requires the user to provide contextFactory.
    /// This way DbContext implementations with any custom constructors can be used.
    /// Example: "FileBasedContextFactor.New(opts => new MyContext(opts)"
    /// </summary>
    /// <returns>The <see cref="IDbContextFactory{TContext}"/>.</returns>
    public static async Task<FileBasedContextFactory<TCtx>> New(Func<DbContextOptions<TCtx>, TCtx> contextFactory)
    {
        var opts = new DbContextOptionsBuilder<TCtx>();
        var filePath = Path.GetTempFileName();
        opts.UseSqlite($"Data Source={filePath}.sqlite");
        var factory =  new FileBasedContextFactory<TCtx>(opts.Options, filePath, contextFactory);
        await using var ctx = await factory.CreateDbContextAsync();
        await ctx.Database.EnsureDeletedAsync();
        await ctx.Database.EnsureCreatedAsync();
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

    /// <inheritdoc />
    public ValueTask DisposeAsync()
    {
        try
        {
            Cleanup();
        }
        catch (Exception e)
        {
            // ignored
            return ValueTask.FromException(e);
        }
        return ValueTask.CompletedTask;
    }

    private void Cleanup() => File.Delete(_filePath);
}
