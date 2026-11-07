using System.Reactive.Linq;
namespace SystemTests;

public class MarketDataSnapshot(ITestOutputHelper output, TestFixture fixture) : CollectionTestBase(output, fixture)
{
    [Fact]
    public async Task MarketDataSnapshotTestAsync()
    {
        Contract contract = new()
        {
            SecurityType = ContractSecurityType.Stock,
            Symbol = "TSLA",
            Currency = "USD",
            Exchange = "SMART"
        };

        IHasRequestId[] messages = await Client.Service.GetMarketDataSnapshotAsync(contract, ct: TestContext.Current.CancellationToken);
        foreach (IHasRequestId message in messages)
            Write(message.Stringify());

        Alert? alert = messages.OfType<Alert>().SingleOrDefault();
        Assert.Null(alert);

        TickRequestParams? trp = messages.OfType<TickRequestParams>().SingleOrDefault();
        Assert.NotNull(trp);

        MarketDataTypeMessage? mdtm = messages.OfType<MarketDataTypeMessage>().SingleOrDefault();
        Assert.NotNull(mdtm);

        IEnumerable<TickBase> ticks = messages.Where(m => m is not TickRequestParams and not MarketDataTypeMessage).Cast<TickBase>();
        Assert.NotEmpty(ticks);
    }

    [Fact]
    public async Task MarketDataSnapshotErrorAsync()
    {
        Contract contract = new()
        {
            SecurityType = ContractSecurityType.Stock,
            Symbol = "InvalidSymbol3",
            Currency = "USD",
            Exchange = "SMART"
        };

        IHasRequestId[] messages = await Client.Service.GetMarketDataSnapshotAsync(contract, ct: TestContext.Current.CancellationToken);
        foreach (IHasRequestId message in messages)
            Write(message.Stringify());

        Assert.Single(messages);
        Alert alertMessage = Assert.IsType<Alert>(messages[0]);
        Assert.StartsWith("No security definition has been found", alertMessage.Message);
    }
}
