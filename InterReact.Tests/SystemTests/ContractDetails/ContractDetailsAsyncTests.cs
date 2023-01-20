using System.Reactive.Linq;

namespace Contracts;

public class ContractDetailsAsync : TestCollectionBase
{
    public ContractDetailsAsync(ITestOutputHelper output, TestFixture fixture) : base(output, fixture) { }
 
    [Fact]
    public async Task SingleTest()
    {
        Contract contract = new() 
        { 
            SecurityType = SecurityType.Stock, 
            Symbol = "MSFT", 
            Currency = "USD", 
            Exchange = "SMART" 
        };

        List<ContractDetails> cds = await Client
            .Service
            .GetContractDetailsAsync(contract);

        ContractDetails cd = Assert.Single(cds);
        Assert.Equal("MSFT", cd.Contract.Symbol);
    }

    [Fact]
    public async Task MultiTest()
    {
        Contract contract = new() 
        { 
            SecurityType = SecurityType.Stock, 
            Symbol = "MSFT", 
            Currency = "USD" 
        };

        List<ContractDetails> cds = await Client
            .Service
            .GetContractDetailsAsync(contract);

        Assert.True(cds.All(cd => cd is ContractDetails)); // no Alerts
        Assert.True(cds.Count > 1); // multiple exchanges
    }

    [Fact]
    public async Task InvalidTest()
    {
        Contract contract = new()
        { 
            ContractId = 99999
        };

        await Assert.ThrowsAsync<AlertException>(async () =>
            await Client
             .Service
             .GetContractDetailsAsync(contract));
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
            .GetContractDetailsAsync(contract)
            .WaitAsync(TimeSpan.FromMilliseconds(1)));
    }
}
