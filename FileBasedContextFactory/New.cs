using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace TestingFixtures;

public partial class FileBasedContextFactory<TCtx>
{
    /// <summary>
    /// Initializes the <see cref="IDbContextFactory{TContext}"/>. Requires the user to provide contextFactory.
    /// This way DbContext implementations with any custom constructors can be used.
    /// Example: "FileBasedContextFactor.New(opts => new MyContext(opts)"
    /// </summary>
    /// <returns>The <see cref="IDbContextFactory{TContext}"/>.</returns>
    public static FileBasedContextFactory<TCtx> New(Func<DbContextOptions<TCtx>, TCtx> contextFactory)
    {
        var opts = SqliteOptions();
        var factory = new FileBasedContextFactory<TCtx>(opts.Options, opts.FilePath, contextFactory);
        EnsureSeededDb(factory);
        return factory;
    }

    /// <summary>
    /// Initializes the <see cref="IDbContextFactory{TContext}"/>. Requires the user to provide contextFactory.
    /// This way DbContext implementations with any custom constructors can be used.
    /// Example: "FileBasedContextFactor.New(opts => new MyContext(opts)"
    /// </summary>
    /// <returns>The <see cref="IDbContextFactory{TContext}"/>.</returns>
    public static async Task<FileBasedContextFactory<TCtx>> NewAsync(Func<DbContextOptions<TCtx>, TCtx> contextFactory)
    {
        var opts = SqliteOptions();
        var factory = new FileBasedContextFactory<TCtx>(opts.Options, opts.FilePath, contextFactory);
        await EnsureSeededDbAsync(factory);
        return factory;
    }

    /// <summary>
    /// Initializes the <see cref="IDbContextFactory{TContext}"/> by trying to find a suitable constructor via reflection.
    /// </summary>
    /// <returns>The <see cref="IDbContextFactory{TContext}"/>.</returns>
    /// <remarks>DbContext is expected to implement a ctor like: DbContext(DbContextOptions options) </remarks>
    public static FileBasedContextFactory<TCtx> New()
    {
        var opts = SqliteOptions();
        var factory = new FileBasedContextFactory<TCtx>(opts.Options, opts.FilePath, CtxFactoryViaReflection(opts.Options));
        EnsureSeededDb(factory);
        return factory;
    }
    
    /// <summary>
    /// Initializes the <see cref="IDbContextFactory{TContext}"/> by trying to find a suitable constructor via reflection.
    /// </summary>
    /// <returns>The <see cref="IDbContextFactory{TContext}"/>.</returns>
    /// <remarks>DbContext is expected to implement a ctor like: DbContext(DbContextOptions options) </remarks>
    public static async Task<FileBasedContextFactory<TCtx>> NewAsync()
    {
        var opts = SqliteOptions();
        var factory = new FileBasedContextFactory<TCtx>(opts.Options, opts.FilePath, CtxFactoryViaReflection(opts.Options));
        await EnsureSeededDbAsync(factory);
        return factory;
    }
    
    private static async Task EnsureSeededDbAsync(FileBasedContextFactory<TCtx> factory)
    {
        await using var ctx = await factory.CreateDbContextAsync();
        await ctx.Database.EnsureDeletedAsync();
        await ctx.Database.EnsureCreatedAsync();
    }

    private static void EnsureSeededDb(FileBasedContextFactory<TCtx> factory)
    {
        var ctx = factory.CreateDbContext();
        ctx.Database.EnsureDeleted();
        ctx.Database.EnsureCreated();
    }

    private static (DbContextOptions<TCtx> Options, string FilePath) SqliteOptions()
    {
        var opts = new DbContextOptionsBuilder<TCtx>();
        var filePath = Path.GetTempFileName();
        opts.UseSqlite($"Data Source={filePath}.sqlite");
        return (Options: opts.Options, FilePath: filePath);
    }

    private static Func<DbContextOptions<TCtx>, TCtx> CtxFactoryViaReflection(DbContextOptions<TCtx> options)
    {
        // run it once to ensure if reflection fails early.
        Type type = typeof(TCtx);
        ConstructorInfo? ctor = type.GetConstructor(new[] { typeof(DbContextOptions<TCtx>) });
        object? instance = ctor?.Invoke(new object[] { options });
        _ = instance as TCtx ?? throw new InvalidOperationException(
            $"Reflection failed. Could not locate ctor. Just provide the ContextFactory manually. ex: '{nameof(FileBasedContextFactory<TCtx>)}<MyCtx>.New(opt => MyCtx(opt))'");
        return (opts) => (TCtx)ctor?.Invoke(new object[] { opts })!;
    }
}