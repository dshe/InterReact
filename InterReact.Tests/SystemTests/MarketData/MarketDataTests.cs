using InterReact;
using System;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;

namespace MarketData;

public class MarketData : TestCollectionBase
{
    public MarketData(ITestOutputHelper output, TestFixture fixture) : base(output, fixture) { }

    [Fact]
    public async Task TicksTest()
    {
        Contract contract = new()
        { 
            SecurityType = SecurityType.Stock, 
            Symbol = "SPY", 
            Currency = "USD", 
            Exchange = "SMART" 
        };

        int id = Client.Request.GetNextId();

        Task<IList<object>> task = Client
            .Response
            .WithRequestId(id)
            .Take(TimeSpan.FromSeconds(3))
            .ToList()
            .ToTask();

        Client.Request.RequestMarketData(id, contract, isSnapshot: false);

        IList<object> messages = await task;

        Assert.Empty(messages.OfType<Alert>().Where(a => a.IsFatal));

        double? lastPrice = messages
                  .OfType<PriceTick>()
                  .Where(x => x.TickType == TickType.DelayedLastPrice)
                  .FirstOrDefault()
                  ?.Price;
        
        Write("LastPrice: " + lastPrice);

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

        int id = Client.Request.GetNextId();

        Task<IList<object>> task = Client
            .Response
            .WithRequestId(id)
            .Take(TimeSpan.FromSeconds(2))
            .ToList()
            .ToTask();

        Client.Request.RequestMarketData(id, contract, isSnapshot: false);

        IList<object> messages = await task;
 
        Alert alert = messages.OfType<Alert>().Single();
        Assert.True(alert.IsFatal);
        Write(alert.Message);

        Assert.StartsWith("No security definition has been found", alert.Message);
    }
}
