namespace SystemTests;

public class ContractDetail(ITestOutputHelper output, TestFixture fixture) : CollectionTestBase(output, fixture)
{
    [Fact]
    public async Task ContractDetailsTestAsync()
    {
        Contract contract = new()
        {
            SecurityType = ContractSecurityType.Stock,
            Symbol = "C",
            Currency = "USD"
        };

        IHasRequestId[] messages = await Client.Service.GetContractDetailsAsync(contract, false, TestContext.Current.CancellationToken);
        foreach (IHasRequestId message in messages)
            Write(message.Stringify());

        Alert? alert = messages.OfType<Alert>().SingleOrDefault();
        Assert.Null(alert);

        ContractDetails[]? cds = null;
        if (alert == null)
            cds = messages.Cast<ContractDetails>().ToArray();
        Assert.NotNull(cds);
        Assert.NotEmpty(cds);
    }

    [Fact]
    public async Task ContractsDetailUnknownSymbolAsync()
    {
        Contract contract = new()
        {
            SecurityType = ContractSecurityType.Stock,
            Symbol = "UnknownSymbol1"
        };

        IHasRequestId[] messages = await Client.Service.GetContractDetailsAsync(contract, false, TestContext.Current.CancellationToken);
        foreach (IHasRequestId message in messages)
            Write(message.Stringify());

        Alert alert = messages.Cast<Alert>().Single();
        Assert.Equal(200, alert.Code);
    }

    [Fact]
    public async Task ContractDetailsErrorAsync()
    {
        Contract contract = new()
        {
            SecurityType = ContractSecurityType.Stock
        };

        IHasRequestId[] messages = await Client.Service.GetContractDetailsAsync(contract, false, TestContext.Current.CancellationToken);
        foreach (IHasRequestId message in messages)
            Write(message.Stringify());

        Alert alert = messages.Cast<Alert>().Single();
        Assert.Equal(321, alert.Code);
    }

    [Fact]
    public async Task ContractDetailsCancellationAsync()
    {
        Contract contract = new()
        {
            SecurityType = ContractSecurityType.Stock,
            Symbol = "A",
            Currency = "USD",
            Exchange = "SMART"
        };

        CancellationToken ct = new(true); // cancelled

        TaskCanceledException ex = await Assert.ThrowsAsync<TaskCanceledException>(async () => await Client.Service.GetContractDetailsAsync(contract, false, ct));

        Write(ex.Message);
    }
}
