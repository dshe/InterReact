using System.Reactive.Linq;

namespace Contracts;

public class ContractDetailService : TestCollectionBase
{
    public ContractDetailService(ITestOutputHelper output, TestFixture fixture) : base(output, fixture) { }
 
    [Fact]
    public async Task SingleTest()
    {
        Contract contract = new() 
        { 
            SecurityType = SecurityType.Stock, 
            Symbol = "IBM", 
            Currency = "USD", 
            Exchange = "SMART" 
        };

        IList<IHasRequestId> messages = await Client
            .Service
            .CreateContractDetailsObservable(contract)
            .ToList();

        IHasRequestId message = Assert.Single(messages);
        ContractDetails cd = Assert.IsType<ContractDetails>(message);
        Assert.Equal("IBM", cd.Contract.Symbol);
    }

    [Fact]
    public async Task MultiTest()
    {
        Contract contract = new() 
        { 
            SecurityType = SecurityType.Stock, 
            Symbol = "IBM", 
            Currency = "USD" 
        };

        IList<IHasRequestId> messages = await Client
            .Service
            .CreateContractDetailsObservable(contract)
            .ToList();

        Assert.True(messages.All(x => x is ContractDetails)); // no Alerts

        Assert.True(messages.Count > 1); // multiple exchanges
    }

    [Fact]
    public async Task InvalidTest()
    {
        Contract contract = new()
        { 
            ContractId = 99999
        };

        IList<IHasRequestId> messages = await Client
             .Service
             .CreateContractDetailsObservable(contract)
             .ToList();

        IHasRequestId message = Assert.Single(messages);
        Alert alert = Assert.IsType<Alert>(message);
        Assert.Equal(200, alert.Code);
    }

    [Fact]
    public async Task TimeoutTest()
    {
        Contract contract = new()
        {
            SecurityType = SecurityType.Stock, 
            Symbol = "IBM", 
            Currency = "EUR" 
        };

        await Assert.ThrowsAsync<TimeoutException>(async () => await Client
            .Service
            .CreateContractDetailsObservable(contract)
            .Timeout(TimeSpan.Zero));
    }
}
