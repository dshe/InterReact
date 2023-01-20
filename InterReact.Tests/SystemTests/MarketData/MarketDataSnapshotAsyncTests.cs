using System.Reactive.Linq;

namespace MarketData;

public class MarketDataSnapshotAsync : TestCollectionBase
{
    public MarketDataSnapshotAsync(ITestOutputHelper output, TestFixture fixture) : base(output, fixture) { }

    [Fact]
    public async Task TickSnapshotAsyncTest()
    {
        Client.Request.RequestMarketDataType(MarketDataType.Delayed);

        Contract contract = new()
        { 
            SecurityType = SecurityType.Stock, 
            Symbol = "IBM", 
            Currency = "USD", 
            Exchange = "SMART" 
        };

        IList<ITick> ticks = await Client
            .Service
            .GetTickSnapshotAsync(contract);

        Assert.NotEmpty(ticks);
    }

    [Fact]
    public async Task TickSnapshotAsyncInvalidTest()
    {
        Client.Request.RequestMarketDataType(MarketDataType.Delayed);

        Contract contract = new()
        { 
            SecurityType = SecurityType.Stock, 
            Symbol = "InvalidSymbol", 
            Currency = "USD", 
            Exchange = "SMART" 
        };

        AlertException akert = await
            Assert.ThrowsAsync<AlertException>(async () => await Client
                .Service
                .GetTickSnapshotAsync(contract));
    }
}
