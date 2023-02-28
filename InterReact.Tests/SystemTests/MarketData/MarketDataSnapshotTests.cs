using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;

namespace MarketData;

public class MarketDataSnapshot : TestCollectionBase
{
    public MarketDataSnapshot(ITestOutputHelper output, TestFixture fixture) : base(output, fixture) { }

    [Fact]
    public async Task TickSnapshotTest()
    {
        Contract contract = new()
        { 
            SecurityType = SecurityType.Stock, 
            Symbol = "AMD", 
            Currency = "USD", 
            Exchange = "SMART" 
        };

        int id = Client.Request.GetNextId();

        Task<IList<object>> task = Client
            .Response
            .WithRequestId(id)
            .TakeUntil(x => x is SnapshotEndTick || (x is AlertMessage alert && alert.IsFatal))
            .ToList()
            .ToTask(); // start task

        Client.Request.RequestMarketData(id, contract, isSnapshot: true);

        IList<object> messages = await task;

        Assert.Empty(messages.OfType<AlertMessage>().Where(a => a.IsFatal));

        double? lastPrice = messages
            .OfType<PriceTick>()
            .FirstOrDefault(x => x.TickType == TickType.DelayedLastPrice || x.TickType == TickType.LastPrice)
            ?.Price;

        Write("LastPrice: " + lastPrice);

        Assert.True(lastPrice != null && lastPrice > 0);
    }

    [Fact]
    public async Task TickSnapshotInvalidTest()
    {
        Contract contract = new()
        { 
            SecurityType = SecurityType.Stock, 
            Symbol = "InvalidSymbol",
            Currency = "USD",
            Exchange = "SMART"
        };

        int id = Client.Request.GetNextId();

        Task<AlertMessage> task = Client
            .Response
            .WithRequestId(id)
            .OfType<AlertMessage>()
            .Where(x => x.IsFatal)
            .FirstAsync()
            .ToTask(); // start task

        Client.Request.RequestMarketData(id, contract, isSnapshot: true);

        AlertMessage fatalAlert = await task;
 
        Assert.True(fatalAlert.Message.StartsWith("No security definition"));
    }
}
