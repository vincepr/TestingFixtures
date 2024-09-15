# TestingFixture
Quickly setup tests for your .NET-EntityFrameWorkCore classes that use DbContext directly or IDbContextFactory<DbContext> directly.

## Repository
* https://github.com/vincepr/TestingFixtures

## Benefits
* Access real databases in your UnitTests by using Sqlite-via-Files or Postgresql-via-Docker.
* Both Sqlite and TestContainers will run in CI-CD-Pipelines (as tests-step in this project's workflow should show)
* The InMemoryDb keeps Items and References in context longer than in production and might introduce subtle Bugs in your Unit Tests.
* Sqlite is still really fast, even compared to the InMemoryDb-Variant
* Postgres via Docker will create a real Database as used in production. So even extra logic like triggers, custom-database-functions etc. can be tested.
* Can be used in a parallelized-way for faster tests.

# Usage
## Examples
- Using The `FileBasedContextFactory<TCtx>` or the `PostgresDockerContextFactory<TCtx>` it is possible to directly
test against a real database. Independent of Nunit.
```csharp
public class SomeProcessWithDbAccessTests
{
    [Test]
    public async Task Article_CorrectlyAdded()
    {
        // Arrange
        using var contextFactory = FileBasedContextFactory<MyDbcontext>.New();
        var articleToAdd = new ArticleDto{ Ean = "22222222", Title = "Pair of wool gloves, red"}
        
        // Act
        new SomeProcessWithDbAccess(contextFactory).AddArticle(articleToAdd);
        
        // Assert
        contextFactory.CreateDbContext().Articles
            .Should().ContainSingle(a => a.Ean == "22222222" && Title == "Pair of wool gloves, red" )
    }
}
```

- When using Nunit there is the convenience implementation for sqlite: `FileBasedTestFixture<TCtx>` and posgres: `PostgresDockerTestFixture<TCtx>` using a TestFixture.
```csharp
public class SomeProcessWithDbAccessTests : FileBasedTestFixture<MyDbContext>
{
    [Test]
    public async Task Article_CorrectlyAdded()
    {
        // Arrange
        var articleToAdd = new ArticleDto{ Ean = "22222222", Title = "Pair of wool gloves, red"}
        
        // Act
        new SomeProcessWithDbAccess(ContextFactory).AddArticle(articleToAdd);
        
        // Assert
        ContextFactory.CreateDbContext().Articles
            .Should().ContainSingle(a => a.Ean == "22222222" && Title == "Pair of wool gloves, red" )
    }
}
```

### Context for the above Examples

- For the no-setup reflection based .New() function to work your custom DbContext is expected
  to have a constructor taking in `(DbContextOptions options)` or `(DbContextOptions<MyDbContext> options)`
- for an example where the DbContext constructor requires check out the Tests.CustomDb folder in the github-repository
```csharp
public class MyDbContext : DbContext
{
    public CustomDbContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<Article> Articles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Article>().HasData(
            new Article
            {
                Ean = "16556324",
                Title = "Sound absorbing dog bed",
            },
            new Article
            {
                Ean = "80295631",
                Title = "Birdhouse Wood",
            }
        );
    }
    
    public record Article
    {
        [Key] public string Ean { get; set; }
        public string Title { get; set; }
    }
}
```

- We have some Repository or Process that directly accesses the db. That we intend to test directly or in a
  combined integration test scenario.
```csharp
public class SomeProcessWithDbAccess(IDbContextFactory<MyDbContext> _contextFactory)
{
    public async Task AddArticle(ArticleDto article)
    {
        await using var ctx = await _contextFactory.CreateDbContextAsync();
        ctx.Articles.Add(new Article
        {
            Ean = article.Title,
            Title = article.Ean,
        })
        await ctx.SaveChangesAsync();
    }
}
```

