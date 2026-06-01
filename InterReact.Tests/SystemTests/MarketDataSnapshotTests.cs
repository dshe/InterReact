using Stringification;
using System;
using System.Reactive.Linq;
namespace SystemTests;

public class MarketDataSnapshot(ITestOutputHelper output, TestFixture fixture) : CollectionTestBase(output, fixture)
{
    [Fact]
    public async Task MarketDataSnapshotTest()
    {
        Contract contract = new()
        {
            SecurityType = ContractSecurityType.Stock,
            Symbol = "NVDA",
            Currency = "USD",
            Exchange = "SMART"
        };

        IObservable<IHasRequestId> observable = Client.Service.CreateMarketDataSnapshotObservable(contract);

        IHasRequestId[] ticks = await observable.ToArray();

        Assert.NotEmpty(ticks);

        //Assert.All(ticks, t => Assert.IsNotType<Alert>(t));

        foreach (var tick in ticks)
            Write(tick.Stringify());
    }

    [Fact]
    public async Task MarketDataSnapshotErrorTest()
    {
        Contract contract = new()
        {
            SecurityType = ContractSecurityType.Stock,
            Symbol = "InvalidSymbol",
            Currency = "USD",
            Exchange = "SMART"
        };

        IObservable<IHasRequestId> observable = Client.Service.CreateMarketDataSnapshotObservable(contract);

        IHasRequestId msg = await observable.FirstAsync();

        Alert alertMessage = Assert.IsType<Alert>(msg);
        
        Assert.StartsWith("No security definition has been found", alertMessage.Message);
    }
}
