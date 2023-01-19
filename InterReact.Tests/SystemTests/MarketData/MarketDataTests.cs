using System.Reactive.Linq;

namespace MarketData;

public class MarketData : TestCollectionBase
{
    public MarketData(ITestOutputHelper output, TestFixture fixture) : base(output, fixture) { }

    private async Task<List<IHasRequestId>> MakeRequest(Contract contract)
    {
        int requestId = Client.Request.GetNextId();

        List<IHasRequestId> messages = new();

        var subscription = Client
            .Response
            .OfType<IHasRequestId>()
            .Where(x => x.RequestId == requestId)
            .Subscribe(m => messages.Add(m));

        Client.Request.RequestMarketDataType(MarketDataType.Delayed);

        Client.Request.RequestMarketData(requestId, contract, isSnapshot: false);

        await Task.Delay(2000);

        subscription.Dispose();

        return messages;
    }

    [Fact]
    public async Task TicksTest()
    {
        Contract contract = new()
        { SecurityType = SecurityType.Stock, Symbol = "IBM", Currency = "USD", Exchange = "SMART" };

        List<IHasRequestId> messages = await MakeRequest(contract);

        Assert.NotEmpty(messages);
        Alert alert = messages.OfType<Alert>().Single();
        Write(alert.Message);
        Assert.False(alert.IsFatal);
        Assert.Equal(messages.Count() - 1, messages.OfType<Tick>().Count());
    }

    [Fact]
    public async Task TicksInvalidTest()
    {
        Contract contract = new()
        { SecurityType = SecurityType.Stock, Symbol = "InvalidSymbol", Currency = "USD", Exchange = "SMART" };

        List<IHasRequestId> messages = await MakeRequest(contract);

        var m = messages.Single();

        Alert alert = messages.OfType<Alert>().Single();
        Assert.True(alert.IsFatal);
        Write(alert.Message);
    }
}
