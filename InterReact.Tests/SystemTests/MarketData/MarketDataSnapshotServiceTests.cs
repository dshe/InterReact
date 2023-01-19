using System.Reactive.Linq;

namespace MarketData;

public class MarketDataSnapshotService : TestCollectionBase
{
    public MarketDataSnapshotService(ITestOutputHelper output, TestFixture fixture) : base(output, fixture) { }

    [Fact]
    public async Task TickSnapshotTest()
    {
        Contract contract = new()
        { SecurityType = SecurityType.Stock, Symbol = "IBM", Currency = "USD", Exchange = "SMART" };

        Client.Request.RequestMarketDataType(MarketDataType.Delayed);

        IList<IHasRequestId> ticks = await Client
            .Service
            .CreateTickSnapshotObservable(contract)
            .ToList();

        Assert.NotEmpty(ticks);
    }

    [Fact]
    public async Task TickSnapshotInvalidTest()
    {
        Contract contract = new()
        { SecurityType = SecurityType.Stock, Symbol = "InvalidSymbol", Currency = "USD", Exchange = "SMART" };

        Client.Request.RequestMarketDataType(MarketDataType.Delayed);

        IList<IHasRequestId> list = await Client
                .Service
                .CreateTickSnapshotObservable(contract)
                .ToList();

        Alert alert = (Alert)list.Single();

        Write("Alert Mesaage: " + alert.Message);
    }
}
