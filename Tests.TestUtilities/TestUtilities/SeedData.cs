using FluentAssertions;
using TestUtilities.DatabaseContexts;

namespace TestUtilities.TestUtilities;

public static class SeedData
{
    public static void AssertCorrectData(List<Article> articles)
    {
        articles.Should().HaveCount(2);
        var a1 = articles.First();
        a1.Ean.Should().Be("16556324");
        a1.Title.Should().Be("Sound absorbing dog bed");
        a1.Prices.Should().HaveCount(2);
        a1.Prices.First().Should().Match<Price>(p => 
            p.Country == Country.DE 
            && p.Currency == CountryCurrency.EUR
            && p.Value == 49.90M);
        a1.Prices.Last().Should().Match<Price>(p => 
            p.Country == Country.GB 
            && p.Currency == CountryCurrency.GBP
            && p.Value == 55.55M);
        var a2 = articles.Last();
        a2.Ean.Should().Be("80295631");
        a2.Title.Should().Be("Birdhouse Wood");
        a2.Prices.Should().HaveCount(1);
        a2.Prices.Single().Should().Match<Price>(p => 
            p.Country == Country.DE 
            && p.Currency == CountryCurrency.EUR
            && p.Value == 11.10M);
    }
    
}