using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;

namespace MarketData;

public class MarketDataSnapshot : TestCollectionBase
{
    public MarketDataSnapshot(ITestOutputHelper output, TestFixture fixture) : base(output, fixture) { }

    private async Task<IList<IHasRequestId>> MakeRequest(Contract contract)
    {
        int requestId = Client.Request.GetNextId();

        var task = Client
            .Response
            .OfType<IHasRequestId>()
            .Where(x => x.RequestId == requestId)
            .TakeUntil(x => x is SnapshotEndTick || (x is Alert alert && alert.IsFatal))
            .Where(x => x is not SnapshotEndTick)
            .ToList()
            .ToTask(); // start task

        Client.Request.RequestMarketDataType(MarketDataType.Delayed);

        Client.Request.RequestMarketData(requestId, contract, isSnapshot: true);

        IList<IHasRequestId> messages = await task;

        Alert? alert = messages.OfType<Alert>().Where(alert => alert.IsFatal).FirstOrDefault();
        if (alert is not null)
            throw new AlertException(alert);

        return messages;
    }

    [Fact]
    public async Task TickSnapshotTest()
    {
        Contract contract = new()
        { SecurityType = SecurityType.Stock, Symbol = "IBM", Currency = "USD", Exchange = "SMART" };

        IList<IHasRequestId> messages = await MakeRequest(contract);

        Assert.True(messages.Count > 1);
    }

    [Fact]
    public async Task TickSnapshotInvalidTest()
    {
        Contract contract = new()
        { SecurityType = SecurityType.Stock, Symbol = "InvalidSymbol", Currency = "USD", Exchange = "SMART" };

        var alertException = await Assert.ThrowsAsync<AlertException>(async () => await MakeRequest(contract));

        Write(alertException.Message);
    }
}
