using Microsoft.EntityFrameworkCore;
using TestUtilities.DatabaseContexts;

namespace Tests.CustomDb.DatabaseContexts;

public class CustomDbContext : DbContext
{
    private readonly uint _someStupidConfig;
    // note:
    // * both SimpleDbContext(DbContextOptions opts) or SimpleDbContext(DbContext<OptionsSimpleDbContext> opts)
    // * will work equally with with Reflection implementation of the New() of the Testing-ContextFactories.
    public CustomDbContext(DbContextOptions options, uint someStupidConfig) : base(options)
    {
        _someStupidConfig = someStupidConfig; // so we can set the amount sold for that dog bed...
    }

    public DbSet<Article> Articles { get; set; }
    public DbSet<Sale> Sales { get; set; }
    public DbSet<Price> Prices { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        SeedArticles(modelBuilder);
        SeedPrices(modelBuilder);
        modelBuilder.Entity<Sale>().HasData(new Sale
        {
            Id = 1,
            SalePrice = 55.55M,
            Amount = _someStupidConfig,
            ArticleEan = "16556324"
        });
    }

    private static void SeedArticles(ModelBuilder modelBuilder)
    {
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

    private static void SeedPrices(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Price>().HasData(
            new Price
            {
                Id = 1,
                ArticleEan = "16556324",
                Country = Country.DE,
                Currency = CountryCurrency.EUR,
                Value = 49.90M,
            },
            new Price
            {
                Id = 2,
                ArticleEan = "16556324",
                Country = Country.GB,
                Currency = CountryCurrency.GBP,
                Value = 55.55M,
            },
            new Price
            {
                Id = 3,
                ArticleEan = "80295631",
                Country = Country.DE,
                Currency = CountryCurrency.EUR,
                Value = 11.10M,
            }
        );
    }
}