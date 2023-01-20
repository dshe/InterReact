using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;

namespace MarketData;

public class MarketDataSnapshot : TestCollectionBase
{
    public MarketDataSnapshot(ITestOutputHelper output, TestFixture fixture) : base(output, fixture) { }

    private async Task<IList<IHasRequestId>> MakeSnapshotRequest(Contract contract)
    {
        int requestId = Client.Request.GetNextId();

        var task = Client
            .Response
            .OfType<IHasRequestId>()
            .Where(x => x.RequestId == requestId)
            .TakeUntil(x => x is SnapshotEndTick || (x is Alert alert && alert.IsFatal))
            .Where(x => x is not SnapshotEndTick)
            .UndelayTicks()
            .ToList()
            .ToTask(); // start task

        // Since the demo account does not have data subscriptions, use delayed data.
        Client.Request.RequestMarketDataType(MarketDataType.Delayed);

        Client.Request.RequestMarketData(requestId, contract, isSnapshot: true);

        IList<IHasRequestId> messages = await task;

        // With delayed data, TickTypes will be delayed: 
        // TickType.BidPrice => TickType.DelayedBidPrice.
        // We can change them back to undelayed using UndelayTicks() operator:
        IList<IHasRequestId> m = messages.UndelayTicks().ToList();

        return messages;
    }

    [Fact]
    public async Task TickSnapshotTest()
    {
        Contract contract = new()
        { 
            SecurityType = SecurityType.Stock, 
            Symbol = "C", 
            Currency = "USD", 
            Exchange = "SMART" 
        };

        IList<IHasRequestId> messages = await MakeSnapshotRequest(contract);

        Assert.Empty(messages.OfType<Alert>().Where(a => a.IsFatal));

        double? lastPrice = messages
            .OfType<PriceTick>()
            .Where(x => x.TickType == TickType.LastPrice)
            .FirstOrDefault()
            ?.Price;

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

        IList<IHasRequestId> messages = await MakeSnapshotRequest(contract);
 
        Alert fatalAlert = messages
            .OfType<Alert>()
            .Where(alert => alert.IsFatal)
            .First();
 
        Assert.True(fatalAlert.Message.StartsWith("No security definition"));
    }
}
