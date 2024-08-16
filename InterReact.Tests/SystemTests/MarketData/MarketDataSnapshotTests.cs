using System;
using System.Reactive.Linq;

namespace MarketData;

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

        IList<PriceTick> priceTicks = await observable
            .IgnoreAlertMessage(ErrorResponse.MarketDataNotSubscribed)
            .ThrowAlertMessage()
            //.OfType<PriceTick>()
            .OfTickClass(x => x.PriceTick)
            .ToList();

        foreach (var priceTick in priceTicks)
            Write(priceTick.TickType + ": " + priceTick.Price);
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

        IObservable<IHasRequestId> observable = Client.Service.CreateMarketDataObservable(contract);
        IHasRequestId message = await observable.FirstAsync();

        Assert.True(message is AlertMessage);
        AlertMessage alert = (AlertMessage)message;
        Write(alert.Message);
        Assert.StartsWith("No security definition has been found", alert.Message);
    }
}
