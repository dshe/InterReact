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

        Assert.Empty(messages.OfType<AlertMessage>().Where(a => a.IsFatal));

        double? lastPrice = messages
            .OfType<PriceTick>()
            .FirstOrDefault(x => x.TickType == TickType.DelayedLastPrice || x.TickType == TickType.LastPrice)
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

        AlertMessage fatalAlert = messages
            .OfType<AlertMessage>()
            .First(alert => alert.IsFatal);

        Assert.True(fatalAlert.Message.StartsWith("No security definition"));
    }
}
