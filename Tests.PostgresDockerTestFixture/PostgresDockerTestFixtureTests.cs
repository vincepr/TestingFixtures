using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using TestingFixtures;
using TestUtilities.DatabaseContexts;
using TestUtilities.TestUtilities;

namespace Tests.PostgresDockerTestFixture;

public class PostgresDockerTestFixtureTests : PostgresDockerTestFixture<SimpleDbContext>
{
    private IDbContextFactory<SimpleDbContext> _contextFactory = null!;
    private SimpleDbContext _context = null!;

    [TearDown]
    public void TearDown()
    {
        // ReSharper disable once ConditionalAccessQualifierIsNonNullableAccordingToAPIContract
        _context?.Dispose();
    }
    
    /*
    // Assert construction of the context-factory is working and building. With actual seeded data in the database.
    */
    
    [Test]
    public void GenericFactory_UsingReflectionForConstructor()
    {
        var articles = ContextFactory.CreateDbContext().Articles.Include(a => a.Prices).ToList();
        SeedData.AssertCorrectData(articles);
        _context = ContextFactory.CreateDbContext();
        _contextFactory = ContextFactory;
        ModifyData.AssertModificationPossible(_context, _contextFactory);
    }
}