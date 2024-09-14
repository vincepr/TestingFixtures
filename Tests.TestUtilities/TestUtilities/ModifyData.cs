using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using TestUtilities.DatabaseContexts;

namespace TestUtilities.TestUtilities;

public static class ModifyData
{
    public static async Task AssertModificationPossible(SimpleDbContext context,
        IDbContextFactory<SimpleDbContext> contextFactory)
    {
        // Assert Context working
        context.Articles.Add(new Article
        {
            Ean = "99",
            Title = "99",
        });
        await context.SaveChangesAsync();

        // Assert ContextFactory working
        await using var ctx = await contextFactory.CreateDbContextAsync();
        var articles = await ctx.Articles.Include(a => a.Prices).ToListAsync();
        articles.Should().HaveCount(3);
        articles.Single(a => a.Ean == "99").Prices.Add(new Price
        {
            Country = Country.GB,
            Currency = CountryCurrency.GBP,
            Value = 99.99M,
        });
        articles.Single(a => a.Ean == "16556324").Prices = new List<Price>();
        await ctx.SaveChangesAsync();

        // Assert changes persisted
        await using var ctxWithModification = await contextFactory.CreateDbContextAsync();
        var prices = await ctxWithModification.Prices.Include(p => p.Article).ToListAsync();
        prices.Single(p => p.Article.Ean == "99").Value.Should().Be(99.99M);

        // Assert Delete working
        prices.Should().NotContain(price => price.Article.Ean == "16556324");
    }
}