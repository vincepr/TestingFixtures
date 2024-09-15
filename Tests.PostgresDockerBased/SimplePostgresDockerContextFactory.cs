using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;
using TestingFixtures;
using TestUtilities.DatabaseContexts;

namespace Tests.PostgresDockerBased;

public class SimplePostgresDockerContextFactory : PostgresDockerContextFactory<SimpleDbContext>
{
    protected SimplePostgresDockerContextFactory(DbContextOptions<SimpleDbContext> options,
        Func<DbContextOptions<SimpleDbContext>, SimpleDbContext> ctxFactory,
        PostgreSqlContainer postgreSqlContainer) : base(options, ctxFactory, postgreSqlContainer)
    {
    }
    
    public static Task<PostgresDockerContextFactory<SimpleDbContext>> NewFuncWithoutReflection() 
        => NewAsync(opts => new SimpleDbContext(opts));
}