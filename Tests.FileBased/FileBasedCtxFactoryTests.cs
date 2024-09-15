using Microsoft.EntityFrameworkCore;
using TestingFixtures;
using TestUtilities.DatabaseContexts;
using TestUtilities.TestUtilities;

namespace Tests.FileBased;

public class FileBasedCtxFactoryTests
{
    [TearDown]
    public void TearDown()
    {
        // ReSharper disable once ConditionalAccessQualifierIsNonNullableAccordingToAPIContract
        _context?.Dispose();
    }
    
    /*
    // Assure our solutions can be cast as the interfaces we expect our actual code to use:
    */
    private IDbContextFactory<SimpleDbContext> _contextFactory = null!;
    private SimpleDbContext _context = null!;
    
    
    /*
    // Assert construction of the context-factory is working and building. With actual seeded data in the database.
    */
    
    [Test]
    public async Task GenericFactory_UsingReflectionForConstructor()
    {
        await using var contextFactory = await FileBasedContextFactory<SimpleDbContext>.NewAsync();
        await RunAndAssertTests(contextFactory);
    }

    [Test]
    public async Task GenericFactory_UsingManualConstructor()
    {
        await using var contextFactory = await FileBasedContextFactory<SimpleDbContext>.NewAsync(o => new SimpleDbContext(o));
        await RunAndAssertTests(contextFactory);
    }
    
    [Test]
    public async Task SpecificFactory_UsingReflectionForConstructor()
    {
        await using var contextFactory = await SimpleFileBasedCtxFactory.NewAsync();
        await RunAndAssertTests(contextFactory);
    }

    [Test]
    public async Task SpecificFactory_UsingManualConstructor()
    {
        await using var contextFactory = await SimpleFileBasedCtxFactory.NewAsync(o => new SimpleDbContext(o));
        await RunAndAssertTests(contextFactory);
    }

    [Test]
    public async Task SpecificFactory_ProvidingOwnNewFunc()
    {
        await using var contextFactory = await SimpleFileBasedCtxFactory.NewFuncWithoutReflection();
        await RunAndAssertTests(contextFactory);
    }
    
    private async Task RunAndAssertTests(FileBasedContextFactory<SimpleDbContext> contextFactory)
    {
        var articles = (await contextFactory.CreateDbContextAsync()).Articles.Include(a => a.Prices).ToList();
        SeedData.AssertCorrectSeedData(articles);
        _context = await contextFactory.CreateDbContextAsync();
        _contextFactory = contextFactory;
        await ModifyData.AssertModificationPossible(_context, _contextFactory);
    }
}