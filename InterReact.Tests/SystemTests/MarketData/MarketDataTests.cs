using System.Reactive.Linq;

namespace MarketData;

public class MarketData : TestCollectionBase
{
    public MarketData(ITestOutputHelper output, TestFixture fixture) : base(output, fixture) { }

    private async Task<List<IHasRequestId>> MakeMarketDataRequest(Contract contract)
    {
        int requestId = Client.Request.GetNextId();

        List<IHasRequestId> messages = new();

        IDisposable subscription = Client
            .Response
            .OfType<IHasRequestId>()
            .Where(x => x.RequestId == requestId)
            .Subscribe(m =>
            {
                messages.Add(m);
            });

        Client.Request.RequestMarketDataType(MarketDataType.Delayed);

        Client.Request.RequestMarketData(requestId, contract, isSnapshot: false);

        await Task.Delay(5000);

        subscription.Dispose();

        return messages;
    }

    [Fact]
    public async Task TicksTest()
    {
        Contract contract = new()
        { 
            SecurityType = SecurityType.Stock, 
            Symbol = "X", 
            Currency = "USD", 
            Exchange = "SMART" 
        };

        List<IHasRequestId> messages = await MakeMarketDataRequest(contract);

        Assert.Empty(messages.OfType<Alert>().Where(a => a.IsFatal));

        double? lastPrice = messages
                  .OfType<PriceTick>()
                  .Where(x => x.TickType == TickType.DelayedLastPrice)
                  .FirstOrDefault()
                  ?.Price;

        Assert.True(lastPrice != null && lastPrice > 0);
    }

    [Fact]
    public async Task TicksInvalidTest()
    {
        Contract contract = new()
        { 
            SecurityType = SecurityType.Stock, 
            Symbol = "InvalidSymbol", 
            Currency = "USD", 
            Exchange = "SMART" 
        };

        List<IHasRequestId> messages = await MakeMarketDataRequest(contract);

        IHasRequestId m = messages.Single();

        Alert alert = messages.OfType<Alert>().Single();
        Assert.True(alert.IsFatal);
        Write(alert.Message);
    }
}
