using Microsoft.EntityFrameworkCore;
using TestingFixtures;
using TestUtilities.DatabaseContexts;

namespace Tests.FileBased;

public class SimpleFileBasedCtxFactory : FileBasedContextFactory<SimpleDbContext>
{
    protected SimpleFileBasedCtxFactory(DbContextOptions<SimpleDbContext> options, string filePath,
        Func<DbContextOptions<SimpleDbContext>, SimpleDbContext> ctxFactory) : base(options, filePath, ctxFactory)
    {
    }

    public static Task<FileBasedContextFactory<SimpleDbContext>> NewFuncWithoutReflection() 
        => New(opts => new SimpleDbContext(opts));
}