using System.Reactive.Linq;

namespace MarketData;

public class MarketData(ITestOutputHelper output, TestFixture fixture) : CollectionTestBase(output, fixture)
{
    [Fact]
    public async Task MarketDataTest()
    {
        Contract contract = new()
        {
            SecurityType = ContractSecurityType.Stock,
            Symbol = "AAPL",
            Currency = "USD",
            Exchange = "SMART"
        };

        IObservable<IHasRequestId> observable = Client.Service.CreateMarketDataObservable(contract);
        //IList<IHasRequestId> messages = await observable.Take(TimeSpan.FromSeconds(2)).ToList();

        PriceTick priceTick = await observable
            .IgnoreAlertMessage(ErrorResponse.MarketDataNotSubscribed)
            .ThrowAlertMessage()
            .OfTickClass(x => x.PriceTick)
            .FirstAsync();
       
        Write(priceTick.TickType + ": " + priceTick.Price);
    }

    [Fact]
    public async Task MarketDataErrorTest()
    {
        Contract contract = new()
        {
            SecurityType = ContractSecurityType.Stock,
            Symbol = "InvalidSymbol",
            Currency = "USD",
            Exchange = "SMART"
        };

        IObservable<IHasRequestId> observable = Client.Service.CreateMarketDataObservable(contract);
        //IList<IHasRequestId> messages = await observable.Take(TimeSpan.FromSeconds(2)).ToList();

        IHasRequestId message = await observable.FirstAsync();

        Assert.True(message is AlertMessage);
        AlertMessage alert = (AlertMessage)message;
        Write(alert.Message);
        Assert.StartsWith("No security definition has been found", alert.Message);
    }
}
