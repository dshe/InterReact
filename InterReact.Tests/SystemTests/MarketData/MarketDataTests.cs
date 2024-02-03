using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;

namespace MarketData;

public class MarketData : CollectionTestBase
{
    public MarketData(ITestOutputHelper output, TestFixture fixture) : base(output, fixture) { }

    [Fact]
    public async Task TicksTest()
    {
        Contract contract = new()
        {
            SecurityType = ContractSecurityType.Stock,
            Symbol = "SPY",
            Currency = "USD",
            Exchange = "SMART"
        };

        int id = Client.Request.GetNextId();

        Task<IList<IHasRequestId>> task = Client
            .Response
            .WithRequestId(id)
            .Take(TimeSpan.FromSeconds(3))
            .ToList()
            .ToTask();

        Client.Request.RequestMarketData(id, contract, isSnapshot: false);

        IList<IHasRequestId> messages = await task; // take 3 seconds of messages

        Assert.Empty(messages.OfType<AlertMessage>().Where(a => a.IsFatal));

        double? lastPrice = messages
            .OfType<PriceTick>()
            .FirstOrDefault(m => m.TickType == TickType.DelayedLastPrice || m.TickType == TickType.LastPrice)
            ?.Price;

        Write("LastPrice: " + lastPrice);

        Assert.True(lastPrice != null && lastPrice > 0);
    }

    [Fact]
    public async Task TicksInvalidTest()
    {
        Contract contract = new()
        {
            SecurityType = ContractSecurityType.Stock,
            Symbol = "InvalidSymbol",
            Currency = "USD",
            Exchange = "SMART"
        };

        int id = Client.Request.GetNextId();

        Task<IList<IHasRequestId>> task = Client
            .Response
            .WithRequestId(id)
            .Take(TimeSpan.FromSeconds(2))
            .ToList()
            .ToTask();

        Client.Request.RequestMarketData(id, contract, isSnapshot: false);

        IList<IHasRequestId> messages = await task;

        AlertMessage alert = messages.OfType<AlertMessage>().Single();
        Assert.True(alert.IsFatal);
        Write(alert.Message);

        Assert.StartsWith("No security definition has been found", alert.Message);
    }
}
