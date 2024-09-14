using FluentAssertions;
using TestingFixtures;
using Tests.CustomDb.DatabaseContexts;

namespace Tests.CustomDb;

[Description("CustomDbContext has a special constructor. So instead of reflection the options have to get manually filled.")]
public class CustomFileBasedFixture : FileBasedTestFixture<CustomDbContext>
{
    private const uint AmountSold = 10;
    
    // we override the setup with while providing our custom configuration options.
    [SetUp]
    public override async Task BaseSetUp()
    {
        ContextFactory = await FileBasedContextFactory<CustomDbContext>.New(o => new CustomDbContext(o, AmountSold));
    }
    
    [Test]
    public async Task FileBased_ManuallyConstructing()
    {
        var sales = (await ContextFactory.CreateDbContextAsync()).Sales;
        foreach (var sale in sales)
        {
            Console.WriteLine(sale);
            sale.Amount.Should().Be(AmountSold);
        }

        sales.Should().HaveCount(1);
    }
}