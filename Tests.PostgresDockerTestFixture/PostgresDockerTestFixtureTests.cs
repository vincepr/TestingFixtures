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
    public async Task GenericFactory_UsingReflectionForConstructor()
    {
        var articles = (await ContextFactory.CreateDbContextAsync()).Articles.Include(a => a.Prices).ToList();
        SeedData.AssertCorrectSeedData(articles);
        _context = await ContextFactory.CreateDbContextAsync();
        _contextFactory = ContextFactory;
        await ModifyData.AssertModificationPossible(_context, _contextFactory);
    }
}