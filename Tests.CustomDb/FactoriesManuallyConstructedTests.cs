using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using TestingFixtures;
using Tests.CustomDb.DatabaseContexts;

namespace Tests.CustomDb;

[Description("CustomDbContext has a special constructor. So instead of reflection the options have to get manually filled.")]
public class FactoriesManuallyConstructedTests
{
    private IDbContextFactory<CustomDbContext> _contextFactory = null!;

    [Test]
    public async Task FileBased_ManuallyConstructing()
    {
        const uint amountSold = 10;
        _contextFactory = await FileBasedContextFactory<CustomDbContext>
            .New(opts => new CustomDbContext(opts, amountSold));
        var sales = (await _contextFactory.CreateDbContextAsync()).Sales;
        foreach (var sale in sales)
        {
            Console.WriteLine(sale);
            sale.Amount.Should().Be(amountSold);
        }

        sales.Should().HaveCount(1);
    }
    
    [Test]
    public async Task Docker_ManuallyConstructing()
    {
        const uint amountSold = 10;
        _contextFactory = await PostgresDockerBasedContextFactory<CustomDbContext>
            .New(opts => new CustomDbContext(opts, amountSold));
        var sales = (await _contextFactory.CreateDbContextAsync()).Sales;
        foreach (var sale in sales)
        {
            Console.WriteLine(sale);
            sale.Amount.Should().Be(amountSold);
        }

        sales.Should().HaveCount(1);
    }
}