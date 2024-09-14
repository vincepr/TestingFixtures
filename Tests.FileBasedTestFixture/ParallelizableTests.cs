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
    public void T1()
    {
        var articles = ContextFactory.CreateDbContext().Articles.Include(a => a.Prices).ToList();
        SeedData.AssertCorrectData(articles);
        _context = ContextFactory.CreateDbContext();
        _contextFactory = ContextFactory;
        ModifyData.AssertModificationPossible(_context, _contextFactory);
    }
    
    [Test]
    public void T2()
    {
        var articles = ContextFactory.CreateDbContext().Articles.Include(a => a.Prices).ToList();
        SeedData.AssertCorrectData(articles);
        _context = ContextFactory.CreateDbContext();
        _contextFactory = ContextFactory;
        ModifyData.AssertModificationPossible(_context, _contextFactory);
    }
    
    [Test]
    public void T3()
    {
        var articles = ContextFactory.CreateDbContext().Articles.Include(a => a.Prices).ToList();
        SeedData.AssertCorrectData(articles);
        _context = ContextFactory.CreateDbContext();
        _contextFactory = ContextFactory;
        ModifyData.AssertModificationPossible(_context, _contextFactory);
    }
    
    [Test]
    public void T4()
    {
        var articles = ContextFactory.CreateDbContext().Articles.Include(a => a.Prices).ToList();
        SeedData.AssertCorrectData(articles);
        _context = ContextFactory.CreateDbContext();
        _contextFactory = ContextFactory;
        ModifyData.AssertModificationPossible(_context, _contextFactory);
    }}