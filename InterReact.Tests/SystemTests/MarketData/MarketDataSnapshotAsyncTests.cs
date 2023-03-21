using System.Reactive.Linq;

namespace MarketData;

public class MarketDataSnapshotAsync : TestCollectionBase
{
    public MarketDataSnapshotAsync(ITestOutputHelper output, TestFixture fixture) : base(output, fixture) { }

    [Fact]
    public async Task TickSnapshotObservableTest()
    {
        Contract contract = new()
        { 
            SecurityType = SecurityType.Stock, 
            Symbol = "NVDA", 
            Currency = "USD", 
            Exchange = "SMART" 
        };

        IList<IHasRequestId> messages = await Client
            .Service
            .CreateTickSnapshotObservable(contract)
            .ToList();

        Assert.Empty(messages.OfType<AlertMessage>().Where(a => a.IsFatal));

        double? lastPrice = messages
            .OfType<PriceTick>()
            .FirstOrDefault(x => x.TickType == TickType.DelayedLastPrice || x.TickType == TickType.LastPrice)
            ?.Price;

        Assert.True(lastPrice != null && lastPrice > 0);
    }

    [Fact]
    public async Task TickSnapshotObservableInvalidTest()
    {
        Contract contract = new()
        { 
            SecurityType = SecurityType.Stock, 
            Symbol = "InvalidSymbol", 
            Currency = "USD", 
            Exchange = "SMART" 
        };

        AlertMessage? fatalAlert = await Client
            .Service
            .CreateTickSnapshotObservable(contract)
            .OfType<AlertMessage>()
            .FirstOrDefaultAsync(alert => alert.IsFatal);

        Assert.NotNull(fatalAlert);
        Assert.True(fatalAlert.Message.StartsWith("No security definition"));
    }
}
