﻿using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace TestingFixtures;

/// <summary>
/// Provides a ContextFactory. Each Test-Run will receive its own instance of a fresh sqlite-database.
/// </summary>
/// <typeparam name="TCtx"><see cref="DbContext"/> of the database-schema tests are run against. </typeparam>
/// <remarks>DbContext is expected to implement a ctor like: DbContext(DbContextOptions options) </remarks>
[TestFixture]
[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
public abstract class PostgresDockerTestFixture<TCtx>
    where TCtx : DbContext
{
    /// <summary>
    /// The exposed ContextFactory. Each Test-Run will receive its own instance of a fresh sqlite-database.
    /// </summary>
    /// <remarks>DbContext is expected to implement a ctor like: DbContext(DbContextOptions options) </remarks>
    protected PostgresDockerContextFactory<TCtx> ContextFactory = null!;
    
    /// <summary>
    /// Identifies a method to be called immediately before each test is run. Initializes the database.
    /// </summary>
    [SetUp]
    public virtual async Task BaseSetUp()
    {
        ContextFactory = await PostgresDockerContextFactory<TCtx>.NewAsync();
    }
    
    /// <summary>
    /// The method is guaranteed to be called, even if an exception is thrown. Tears down the database.
    /// </summary>
    [TearDown]
    public void BaseTearDown()
    {
        ContextFactory.Dispose();
    }
}