using ClassLibrary1;
using Microsoft.EntityFrameworkCore;
using TestUtilities.DatabaseContexts;
using TestUtilities.TestUtilities;

namespace Tests.PostgresDockerBased;

public class PostgresDockerBasedContextFactoryTests
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
        await using var contextFactory = await PostgresDockerBasedContextFactory<SimpleDbContext>.New();
        var articles = contextFactory.CreateDbContext().Articles.Include(a => a.Prices).ToList();
        SeedData.AssertCorrectData(articles);
        _context = contextFactory.CreateDbContext();
        _contextFactory = contextFactory;
        ModifyData.AssertModificationPossible(_context, _contextFactory);
    }
    
    [Test]
    public async Task GenericFactory_UsingManualConstructor()
    {
        using var contextFactory = await PostgresDockerBasedContextFactory<SimpleDbContext>.New(o => new SimpleDbContext(o));
        var articles = contextFactory.CreateDbContext().Articles.Include(a => a.Prices).ToList();
        SeedData.AssertCorrectData(articles);
        _context = contextFactory.CreateDbContext();
        _contextFactory = contextFactory;
        ModifyData.AssertModificationPossible(_context, _contextFactory);
    }
    
    [Test]
    public async Task SpecificFactory_UsingReflectionForConstructor()
    {
        using var contextFactory = await SimplePostgresDockerBasedContextFactory.New();
        var articles = contextFactory.CreateDbContext().Articles.Include(a => a.Prices).ToList();
        SeedData.AssertCorrectData(articles);
        _context = contextFactory.CreateDbContext();
        _contextFactory = contextFactory;
        ModifyData.AssertModificationPossible(_context, _contextFactory);
    }

    [Test]
    public async Task SpecificFactory_UsingManualConstructor()
    {
        using var contextFactory = await SimplePostgresDockerBasedContextFactory.New(o => new SimpleDbContext(o));
        var articles = contextFactory.CreateDbContext().Articles.Include(a => a.Prices).ToList();
        SeedData.AssertCorrectData(articles);
        _context = contextFactory.CreateDbContext();
        _contextFactory = contextFactory;
        ModifyData.AssertModificationPossible(_context, _contextFactory);
    }
    
    [Test]
    public async Task SpecificFactory_ProvidingOwnNewFunc()
    {
        using var contextFactory = await SimplePostgresDockerBasedContextFactory.NewFuncWithoutReflection();
        var articles = contextFactory.CreateDbContext().Articles.Include(a => a.Prices).ToList();
        SeedData.AssertCorrectData(articles);
        _context = contextFactory.CreateDbContext();
        _contextFactory = contextFactory;
        ModifyData.AssertModificationPossible(_context, _contextFactory);
    }
}