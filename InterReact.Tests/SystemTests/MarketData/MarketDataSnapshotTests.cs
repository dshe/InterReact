using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;

namespace MarketData;

public class MarketDataSnapshot : CollectionTestBase
{
    public MarketDataSnapshot(ITestOutputHelper output, TestFixture fixture) : base(output, fixture) { }

    [Fact]
    public async Task TickSnapshotTest()
    {
        Contract contract = new()
        {
            SecurityType = ContractSecurityType.Stock,
            Symbol = "AMD",
            Currency = "USD",
            Exchange = "SMART"
        };

        int id = Client.Request.GetNextId();

        Task<IList<IHasRequestId>> task = Client
            .Response
            .WithRequestId(id)
            .TakeUntil(x => x is SnapshotEndTick || (x is AlertMessage alert && alert.IsFatal))
            .ToList()
            .ToTask(); // start task

        Client.Request.RequestMarketData(id, contract, isSnapshot: true);

        IList<IHasRequestId> messages = await task;

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
            SecurityType = ContractSecurityType.Stock,
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
