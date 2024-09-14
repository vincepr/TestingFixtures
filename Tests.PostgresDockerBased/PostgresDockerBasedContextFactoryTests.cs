using Microsoft.EntityFrameworkCore;
using TestingFixtures;
using TestUtilities.DatabaseContexts;
using TestUtilities.TestUtilities;

namespace Tests.PostgresDockerBased;

[Parallelizable]
public class PostgresDockerBasedContextFactoryTests
{
    /*
    // Assert construction of the context-factory is working and building. With actual seeded data in the database.
    */

    [Parallelizable]
    [Test]
    public async Task GenericFactory_UsingReflectionForConstructor()
    {
        await using var contextFactory = await PostgresDockerBasedContextFactory<SimpleDbContext>.New();
        await RunAndAssertTests(contextFactory);
    }

    [Parallelizable]
    [Test]
    public async Task GenericFactory_UsingManualConstructor()
    {
        await using var contextFactory =
            await PostgresDockerBasedContextFactory<SimpleDbContext>.New(o => new SimpleDbContext(o));
        await RunAndAssertTests(contextFactory);
    }

    [Parallelizable]
    [Test]
    public async Task SpecificFactory_UsingReflectionForConstructor()
    {
        await using var contextFactory = await SimplePostgresDockerBasedContextFactory.New();
        await RunAndAssertTests(contextFactory);
    }

    [Parallelizable]
    [Test]
    public async Task SpecificFactory_UsingManualConstructor()
    {
        await using var contextFactory = await SimplePostgresDockerBasedContextFactory.New(o => new SimpleDbContext(o));
        await RunAndAssertTests(contextFactory);
    }

    [Parallelizable]
    [Test]
    public async Task SpecificFactory_ProvidingOwnNewFunc()
    {
        await using var contextFactory = await SimplePostgresDockerBasedContextFactory.NewFuncWithoutReflection();
        await RunAndAssertTests(contextFactory);
    }
    
    private async Task RunAndAssertTests(PostgresDockerBasedContextFactory<SimpleDbContext> testFactory)
    {
        var articles = (await testFactory.CreateDbContextAsync()).Articles.Include(a => a.Prices).ToList();
        SeedData.AssertCorrectSeedData(articles);
        var context = await testFactory.CreateDbContextAsync();
        var factory = testFactory;
        await ModifyData.AssertModificationPossible(context, factory);
    }
}