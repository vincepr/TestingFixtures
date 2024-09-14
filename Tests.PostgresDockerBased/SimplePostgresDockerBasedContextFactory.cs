using ClassLibrary1;
using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;
using TestUtilities.DatabaseContexts;

namespace Tests.PostgresDockerBased;

public class SimplePostgresDockerBasedContextFactory : PostgresDockerBasedContextFactory<SimpleDbContext>
{
    protected SimplePostgresDockerBasedContextFactory(DbContextOptions<SimpleDbContext> options,
        Func<DbContextOptions<SimpleDbContext>, SimpleDbContext> ctxFactory,
        PostgreSqlContainer postgreSqlContainer) : base(options, ctxFactory, postgreSqlContainer)
    {
    }
    
    public static Task<PostgresDockerBasedContextFactory<SimpleDbContext>> NewFuncWithoutReflection() 
        => New(opts => new SimpleDbContext(opts));
}