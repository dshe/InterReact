using System.Reactive.Linq;

namespace Contracts;

public class ContractDetail(ITestOutputHelper output, TestFixture fixture) : CollectionTestBase(output, fixture)
{
    [Fact]
    public async Task ContractDetailSingleTest()
    {
        Contract contract = new()
        {
            SecurityType = ContractSecurityType.Stock,
            Symbol = "MSFT",
            Currency = "USD",
            Exchange = "SMART"
        };

        IList<ContractDetails> messages = await Client
            .Service
            .GetContractDetailsAsync(contract);

        var cd = messages.Single();

        Assert.Equal("MSFT", cd.Contract.Symbol);
    }

    [Fact]
    public async Task ContractDetailMultiTest()
    {
        Contract contract = new()
        {
            SecurityType = ContractSecurityType.Stock,
            Symbol = "MSFT",
            Currency = "USD"
        };

        IList<ContractDetails> cds = await Client
            .Service
            .GetContractDetailsAsync(contract);

        Assert.True(cds.Count > 1); // multiple exchanges
    }

    [Fact]
    public async Task ContractDetailErrorTest()
    {
        Contract contract = new()
        {
            ContractId = 99999
        };

        await Assert.ThrowsAsync<AlertException>(async () => await Client
            .Service
            .GetContractDetailsAsync(contract));
    }

    [Fact]
    public async Task ContractDetailTimeoutTest()
    {
        Contract contract = new()
        {
            SecurityType = ContractSecurityType.Stock,
            Symbol = "X",
            Currency = "USD"
        };

        await Assert.ThrowsAsync<TimeoutException>(async() => await Client
            .Service
            .CreateContractDetailsObservable(contract)
            .Timeout(TimeSpan.FromMilliseconds(1))
            .ToList());
     }
}
