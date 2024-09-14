using FileBased;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace FileBasedTestFixture;

[TestFixture]
public class FileBasedTestFixture<TCtx>
    where TCtx : DbContext
{
    public FileBasedContextFactory<TCtx> ContextFactory;

    [SetUp]
    public async Task BaseSetUp()
    {
        ContextFactory = await FileBasedContextFactory<TCtx>.New();
    }

    [TearDown]
    public void BaseTearDown()
    {
        ContextFactory.Dispose();
    }

}