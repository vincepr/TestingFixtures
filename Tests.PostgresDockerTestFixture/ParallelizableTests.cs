using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using TestUtilities.DatabaseContexts;
using TestUtilities.TestUtilities;

namespace Tests.PostgresDockerTestFixture;

[Parallelizable(ParallelScope.All)]
public class ParallelizableTests : SimplePostgresDockerTestFixture
{
    // note: we know [FixtureLifeCycle(LifeCycle.InstancePerTestCase)] is set  in PostgresDockerTestFixture.
    // so we can safely use these fields in a parallelized context.
    private IDbContextFactory<SimpleDbContext> _contextFactory = null!;
    private SimpleDbContext _context = null!;

    /*
     * These Tests should run in parallel. Every running test get it's own scope because of the [FixtureLifeCycle(LifeCycle.InstancePerTestCase)] 
     */
    
    [Test]
    public async Task T1()
    {
        var articles = (await ContextFactory.CreateDbContextAsync()).Articles.Include(a => a.Prices).ToList();
        SeedData.AssertCorrectSeedData(articles);
        _context = await ContextFactory.CreateDbContextAsync();
        _contextFactory = ContextFactory;
        await ModifyData.AssertModificationPossible(_context, _contextFactory);
    }
    
    [Test]
    public async Task T2()
    {
        var articles = (await ContextFactory.CreateDbContextAsync()).Articles.Include(a => a.Prices).ToList();
        SeedData.AssertCorrectSeedData(articles);
        _context = await ContextFactory.CreateDbContextAsync();
        _contextFactory = ContextFactory;
        await ModifyData.AssertModificationPossible(_context, _contextFactory);
    }
    
    [Test]
    public async Task T3()
    {
        var articles = (await ContextFactory.CreateDbContextAsync()).Articles.Include(a => a.Prices).ToList();
        SeedData.AssertCorrectSeedData(articles);
        _context = await ContextFactory.CreateDbContextAsync();
        _contextFactory = ContextFactory;
        await ModifyData.AssertModificationPossible(_context, _contextFactory);
    }
    
    [Test]
    public async Task T4()
    {
        var articles = (await ContextFactory.CreateDbContextAsync()).Articles.Include(a => a.Prices).ToList();
        SeedData.AssertCorrectSeedData(articles);
        _context = await ContextFactory.CreateDbContextAsync();
        _contextFactory = ContextFactory;
        await ModifyData.AssertModificationPossible(_context, _contextFactory);
    }
    
    [Test]
    public async Task T5()
    {
        var articles = (await ContextFactory.CreateDbContextAsync()).Articles.Include(a => a.Prices).ToList();
        SeedData.AssertCorrectSeedData(articles);
        _context = await ContextFactory.CreateDbContextAsync();
        _contextFactory = ContextFactory;
        await ModifyData.AssertModificationPossible(_context, _contextFactory);
    }
    
    [Test]
    public async Task T6()
    {
        var articles = (await ContextFactory.CreateDbContextAsync()).Articles.Include(a => a.Prices).ToList();
        SeedData.AssertCorrectSeedData(articles);
        _context = await ContextFactory.CreateDbContextAsync();
        _contextFactory = ContextFactory;
        await ModifyData.AssertModificationPossible(_context, _contextFactory);
    }
    
    [Test]
    public async Task T7()
    {
        var articles = (await ContextFactory.CreateDbContextAsync()).Articles.Include(a => a.Prices).ToList();
        SeedData.AssertCorrectSeedData(articles);
        _context = await ContextFactory.CreateDbContextAsync();
        _contextFactory = ContextFactory;
        await ModifyData.AssertModificationPossible(_context, _contextFactory);
    }
    
    [Test]
    public async Task T8()
    {
        var articles = (await ContextFactory.CreateDbContextAsync()).Articles.Include(a => a.Prices).ToList();
        SeedData.AssertCorrectSeedData(articles);
        _context = await ContextFactory.CreateDbContextAsync();
        _contextFactory = ContextFactory;
        await ModifyData.AssertModificationPossible(_context, _contextFactory);
    }
}