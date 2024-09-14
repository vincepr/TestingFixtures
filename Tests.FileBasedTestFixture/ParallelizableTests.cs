using Microsoft.EntityFrameworkCore;
using TestUtilities.DatabaseContexts;
using TestUtilities.TestUtilities;

namespace Tests.FileBasedTestFixture;

[Parallelizable(ParallelScope.All)]
public class ParallelizableTests : SimpleFileBasedTestFixture
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
    }}