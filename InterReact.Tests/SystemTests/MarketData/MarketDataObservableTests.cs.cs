using System.Reactive.Linq;

namespace MarketData;

public class MarketDataObservable : TestCollectionBase
{
    public MarketDataObservable(ITestOutputHelper output, TestFixture fixture) : base(output, fixture) { }

    [Fact]
    public async Task TicksTest()
    {
        Contract contract = new()
        { 
            SecurityType = SecurityType.Stock,
            Symbol = "AAPL", 
            Currency = "USD", 
            Exchange = "SMART" 
        };

        List<object> messages = new();

        IDisposable subscription = Client
            .Service
            .CreateTickObservable(contract)
            .Subscribe(m =>
            {
                messages.Add(m);
            });

        await Task.Delay(2000);

        subscription.Dispose();

        Assert.Empty(messages.OfType<AlertMessage>().Where(a => a.IsFatal));

        double? lastPrice = messages
            .OfType<PriceTick>()
            .FirstOrDefault(x => x.TickType == TickType.DelayedLastPrice)
            ?.Price;

        Assert.True(lastPrice is > 0);
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

        List<object> messages = new();

        IDisposable subscription = Client
            .Service
            .CreateTickObservable(contract)
            .Subscribe(m =>
            {
                messages.Add(m);
            });

        await Task.Delay(2000);

        subscription.Dispose();

        AlertMessage alert = messages
            .OfType<AlertMessage>()
            .First(a => a.IsFatal);

        Assert.True(alert.Message.StartsWith("No security definition"));
    }
}
