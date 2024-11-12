using Stringification;
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
            Symbol = "GOOG",
            Currency = "USD",
            Exchange = "SMART"
        };

        IObservable<IHasRequestId> observable = Client.Service.CreateMarketDataObservable(contract);
        
        IHasRequestId[] messages = await observable.Take(TimeSpan.FromSeconds(3)).ToArray();

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

        var ex = await Assert.ThrowsAsync<AlertException>(async () => await observable.FirstAsync());
        Assert.StartsWith("Alert: No security definition has been found", ex.Message);
    }
}
