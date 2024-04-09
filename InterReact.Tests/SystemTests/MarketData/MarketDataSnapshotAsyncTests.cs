using System.Reactive.Linq;

namespace MarketData;

public class MarketDataSnapshotAsync : CollectionTestBase
{
    public MarketDataSnapshotAsync(ITestOutputHelper output, TestFixture fixture) : base(output, fixture) { }

    [Fact]
    public async Task TickSnapshotObservableTest()
    {
        Contract contract = new()
        {
            SecurityType = ContractSecurityType.Stock,
            Symbol = "NVDA",
            Currency = "USD",
            Exchange = "SMART"
        };

        IList<IHasRequestId> messages = await Client
            .Service
            .GetTickSnapshotAsync(contract);

        Assert.Empty(messages.OfType<AlertMessage>().Where(a => a.IsFatal));

        double? lastPrice = messages
            .OfType<PriceTick>()
            .FirstOrDefault(x => x.TickType == TickType.DelayedLastPrice || x.TickType == TickType.LastPrice)
            ?.Price;

        Assert.True(lastPrice != null && lastPrice > 0);
    }

    [Fact]
    public async Task TickSnapshotObservableInvalidTest()
    {
        Contract contract = new()
        {
            SecurityType = ContractSecurityType.Stock,
            Symbol = "InvalidSymbol",
            Currency = "USD",
            Exchange = "SMART"
        };

        try
        {
            await Client.Service.GetTickSnapshotAsync(contract);
        }
        catch (AlertException alertException)
        {
            //Assert.True(alertException.IsFatal);
            Assert.StartsWith("No security definition", alertException.Message);
        }

        //Assert.NotNull(fatalAlert);
        //Assert.True(fatalAlert.Message.StartsWith("No security definition"));
    }
}
