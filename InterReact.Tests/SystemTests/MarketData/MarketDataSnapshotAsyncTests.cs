using System.Reactive.Linq;

namespace MarketData;

public class MarketDataSnapshotAsync : TestCollectionBase
{
    public MarketDataSnapshotAsync(ITestOutputHelper output, TestFixture fixture) : base(output, fixture) { }

    [Fact]
    public async Task TickSnapshotAsyncTest()
    {
        Contract contract = new()
        { 
            SecurityType = SecurityType.Stock, 
            Symbol = "NVDA", 
            Currency = "USD", 
            Exchange = "SMART" 
        };

        IList<object> messages = await Client
            .Service
            .GetTickSnapshotAsync(contract);

        Assert.Empty(messages.OfType<Alert>().Where(a => a.IsFatal));

        double? lastPrice = messages
             .OfType<PriceTick>()
             .Where(x => x.TickType == TickType.DelayedLastPrice)
             .FirstOrDefault()
             ?.Price;

        Assert.True(lastPrice != null && lastPrice > 0);
    }

    [Fact]
    public async Task TickSnapshotAsyncInvalidTest()
    {
        Contract contract = new()
        { 
            SecurityType = SecurityType.Stock, 
            Symbol = "InvalidSymbol", 
            Currency = "USD", 
            Exchange = "SMART" 
        };

        IList<object> messages = await Client
            .Service
            .GetTickSnapshotAsync(contract);

        Alert fatalAlert = messages
            .OfType<Alert>()
            .Where(alert => alert.IsFatal)
            .First();

        Assert.True(fatalAlert.Message.StartsWith("No security definition"));
    }
}
