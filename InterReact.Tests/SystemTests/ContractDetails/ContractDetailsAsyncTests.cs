namespace Contracts;

public class ContractDetailsAsync : CollectionTestBase
{
    public ContractDetailsAsync(ITestOutputHelper output, TestFixture fixture) : base(output, fixture) { }

    [Fact]
    public async Task SingleTest()
    {
        Contract contract = new()
        {
            SecurityType = ContractSecurityType.Stock,
            Symbol = "MSFT",
            Currency = "USD",
            Exchange = "SMART"
        };

        IList<ContractDetails> cds = await Client
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
    public async Task InvalidTest()
    {
        Contract contract = new()
        {
            ContractId = 99999
        };

        await Assert.ThrowsAsync<AlertException>(() => Client
            .Service
            .GetContractDetailsAsync(contract));
    }

    [Fact]
    public async Task TimeoutTest()
    {
        Contract contract = new()
        {
            SecurityType = ContractSecurityType.Stock,
            Symbol = "X",
            Currency = "USD"
        };

        CancellationTokenSource cts = new();
        cts.CancelAfter(1);

        await Assert.ThrowsAsync<TaskCanceledException>(() => Client
            .Service
            .GetContractDetailsAsync(contract, cts.Token));
    }
}
