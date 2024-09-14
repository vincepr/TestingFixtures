using ClassLibrary1;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace PostgresDockerTestFixture;

[TestFixture]
[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
public class PostgresDockerTestFixture<TCtx>
    where TCtx : DbContext
{
    public PostgresDockerBasedContextFactory<TCtx> ContextFactory;

    [SetUp]
    public async Task BaseSetUp()
    {
        ContextFactory = await PostgresDockerBasedContextFactory<TCtx>.New();
    }

    [TearDown]
    public void BaseTearDown()
    {
        ContextFactory.Dispose();
    }
}