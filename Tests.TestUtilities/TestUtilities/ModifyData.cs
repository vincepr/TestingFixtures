using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using TestUtilities.DatabaseContexts;

namespace TestUtilities.TestUtilities;

public static class ModifyData
{
    public static void AssertModificationPossible(SimpleDbContext context,
        IDbContextFactory<SimpleDbContext> contextFactory)
    {
        // Assert Context working
        context.Articles.Add(new Article
        {
            Ean = "99",
            Title = "99",
        });
        context.SaveChanges();

        // Assert ContextFactory working
        using var ctx = contextFactory.CreateDbContext();
        var articles = ctx.Articles.Include(a => a.Prices).ToList();
        articles.Should().HaveCount(3);
        articles.Single(a => a.Ean == "99").Prices.Add(new Price
        {
            Country = Country.GB,
            Currency = CountryCurrency.GBP,
            Value = 99.99M,
        });
        ctx.SaveChanges();


        // Assert changes persisted
        using var ctxWithModification = contextFactory.CreateDbContext();
        ctxWithModification.Prices
            .Include(p => p.Article)
            .Single(p => p.Article.Ean == "99").Value.Should().Be(99.99M);
    }
}