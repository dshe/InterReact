using System.Reactive.Linq;

namespace MarketData;

public class MarketDataObservable : TestCollectionBase
{
    public MarketDataObservable(ITestOutputHelper output, TestFixture fixture) : base(output, fixture) { }

    private async Task<IList<IHasRequestId>> MakeRequest(Contract contract)
    {
        Client.Request.RequestMarketDataType(MarketDataType.Delayed);

        IList<IHasRequestId> ticks = await Client
            .Service
            .CreateTickObservable(contract)
            .Take(TimeSpan.FromSeconds(2))
            .ToList();

        return ticks;
    }

    [Fact]
    public async Task TicksTest()
    {
        Contract contract = new()
        { SecurityType = SecurityType.Stock, Symbol = "IBM", Currency = "USD", Exchange = "SMART" };

        IList<IHasRequestId> ticks = await MakeRequest(contract);

        Assert.NotEmpty(ticks);
    }

    [Fact]
    public async Task TicksInvalidTest()
    {
        Contract contract = new()
        { SecurityType = SecurityType.Stock, Symbol = "InvalidSymbol", Currency = "USD", Exchange = "SMART" };


        IList<IHasRequestId> list = await MakeRequest(contract);

        IHasRequestId message = list.Single();
        Alert alert = (Alert)message;
;
        Write("Alert Meszsage: " + alert.Message);
    }
}
