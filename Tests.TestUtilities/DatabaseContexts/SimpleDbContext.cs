using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TestUtilities.DatabaseContexts;

public class SimpleDbContext : DbContext
{
    // note:
    // * both SimpleDbContext(DbContextOptions opts) or SimpleDbContext(DbContext<OptionsSimpleDbContext> opts)
    // * will work equally with with Reflection implementation of the New() of the Testing-ContextFactories.
    public SimpleDbContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<Article> Articles { get; set; }
    public DbSet<Sale> Sales { get; set; }
    public DbSet<Price> Prices { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        SeedArticles(modelBuilder);
        SeedPrices(modelBuilder);
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

public class Sale
{
    [Key] public int Id { get; set; }

    /// <summary>
    /// Sale price in Euro.
    /// </summary>
    public decimal SalePrice { get; set; }

    public uint Amount { get; set; }
    public Article Article { get; set; }
    [ForeignKey(nameof(Sale.Article))] public string ArticleEan { get; set; }
}

public record Article
{
    [Key] public string Ean { get; set; }
    public string Title { get; set; }
    public List<Price> Prices { get; set; }
}

public record Price
{
    [Key] public int Id { get; set; }
    public Country Country { get; set; }
    public CountryCurrency Currency { get; set; }
    public decimal Value { get; set; }
    public Article Article { get; set; }
    [ForeignKey(nameof(Sale.Article))] public string ArticleEan { get; set; }
}

public enum Country
{
    DE,
    GB,
}

public enum CountryCurrency
{
    EUR,
    GBP,
}