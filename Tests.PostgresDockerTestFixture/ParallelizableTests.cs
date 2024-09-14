using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using TestUtilities.DatabaseContexts;
using TestUtilities.TestUtilities;

namespace Tests.PostgresDockerTestFixture;

public class ParallelizableTests : SimplePostgresDockerTestFxiture
{
    private IDbContextFactory<SimpleDbContext> _contextFactory = null!;
    private SimpleDbContext _context = null!;

    /*
     * These Tests should run in parallel. Every running test get it's own scope because of the [FixtureLifeCycle(LifeCycle.InstancePerTestCase)] 
     */
    
    [Parallelizable]
    [Test]
    public void T1()
    {
        var articles = ContextFactory.CreateDbContext().Articles.Include(a => a.Prices).ToList();
        SeedData.AssertCorrectData(articles);
        _context = ContextFactory.CreateDbContext();
        _contextFactory = ContextFactory;
        ModifyData.AssertModificationPossible(_context, _contextFactory);
    }
    
    [Parallelizable]
    [Test]
    public void T2()
    {
        var articles = ContextFactory.CreateDbContext().Articles.Include(a => a.Prices).ToList();
        SeedData.AssertCorrectData(articles);
        _context = ContextFactory.CreateDbContext();
        _contextFactory = ContextFactory;
        ModifyData.AssertModificationPossible(_context, _contextFactory);
    }
    
    [Parallelizable]
    [Test]
    public void T3()
    {
        var articles = ContextFactory.CreateDbContext().Articles.Include(a => a.Prices).ToList();
        SeedData.AssertCorrectData(articles);
        _context = ContextFactory.CreateDbContext();
        _contextFactory = ContextFactory;
        ModifyData.AssertModificationPossible(_context, _contextFactory);
    }
    
    [Parallelizable]
    [Test]
    public void T4()
    {
        var articles = ContextFactory.CreateDbContext().Articles.Include(a => a.Prices).ToList();
        SeedData.AssertCorrectData(articles);
        _context = ContextFactory.CreateDbContext();
        _contextFactory = ContextFactory;
        ModifyData.AssertModificationPossible(_context, _contextFactory);
    }}