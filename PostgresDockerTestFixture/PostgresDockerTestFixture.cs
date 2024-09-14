using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace TestingFixtures;

/// <summary>
/// Provides a ContextFactory. Each Test-Run will receive its own instance of a fresh sqlite-database.
/// </summary>
/// <typeparam name="TCtx"><see cref="DbContext"/> of the database-schema tests are run against. </typeparam>
/// <remarks>DbContext is expected to implement a ctor like: DbContext(DbContextOptions options) </remarks>
[TestFixture]
[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
public class PostgresDockerTestFixture<TCtx>
    where TCtx : DbContext
{
    private PostgresDockerContextFactory<TCtx> _contextFactory = null!;
    
    /// <summary>
    /// The exposed ContextFactory. Each Test-Run will receive its own instance of a fresh sqlite-database.
    /// </summary>
    /// <typeparam name="TCtx"><see cref="DbContext"/> of the database-schema tests are run against. </typeparam>
    /// <remarks>DbContext is expected to implement a ctor like: DbContext(DbContextOptions options) </remarks>
    protected IDbContextFactory<TCtx> ContextFactory = null!;
    
    /// <summary>
    /// Identifies a method to be called immediately before each test is run. Initializes the database.
    /// </summary>
    [SetUp]
    public virtual async Task BaseSetUp()
    {
        _contextFactory = await PostgresDockerContextFactory<TCtx>.New();
        ContextFactory = _contextFactory;
    }
    
    /// <summary>
    /// The method is guaranteed to be called, even if an exception is thrown. Tears down the database.
    /// </summary>
    [TearDown]
    public void BaseTearDown()
    {
        _contextFactory.Dispose();
    }
}