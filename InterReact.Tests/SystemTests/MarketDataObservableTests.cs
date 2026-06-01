using Stringification;
using System.Reactive.Linq;
namespace SystemTests;

public class MarketData(ITestOutputHelper output, TestFixture fixture) : CollectionTestBase(output, fixture)
{
    [Fact]
    public async Task MarketDataTest()
    {
        Contract contract = new()
        {
            SecurityType = ContractSecurityType.Stock,
            Symbol = "NVDA",
            Currency = "USD",
            Exchange = "SMART"
        };

        IObservable<IHasRequestId> observable = Client.Service.CreateMarketDataObservable(contract);
        
        IHasRequestId[] messages = await observable.Take(TimeSpan.FromSeconds(5)).ToArray();

        foreach (IHasRequestId m in messages)
            Write(m.Stringify());

        Assert.True(messages.Length > 3);
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

        IHasRequestId msg = await observable.FirstAsync();

        Alert alertMessage = Assert.IsType<Alert>(msg);

        Assert.StartsWith("No security definition has been found", alertMessage.Message);
    }

}
