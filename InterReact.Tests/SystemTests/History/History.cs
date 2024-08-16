using Stringification;
using System.Reactive.Linq;

namespace History;

public class HistoryTests(ITestOutputHelper output, TestFixture fixture) : CollectionTestBase(output, fixture)
{
    [Fact]
    public async Task HistoryObservableTest()
    {
        Contract contract = new()
        {
            SecurityType = ContractSecurityType.Cash,
            PrimaryExchange = "IDEALPRO",
            Symbol = "EUR",
            Currency = "USD",
            Exchange = "SMART"
        };

        IObservable<IHasRequestId> observable = Client
            .Service
            .CreateHistoricalDataObservable(
                contract,
                HistoricalDataDuration.EightHours,
                HistoricalDataBarSize.FiveSeconds,
                HistoricalDataWhatToShow.Midpoint,
                false
            );

        IList<IHasRequestId> messages = await observable
            //.IgnoreAlertMessage(ErrorResponse.HistoricalDataServiceError)
            //.CastTo<HistoricalDataBar>()
            // wait some time for TWS to send some updates
            .Take(TimeSpan.FromSeconds(10))
            .ToList();

        Assert.NotEmpty(messages);
        foreach (HistoricalDataBar bar in messages.OfType<HistoricalDataBar>())
            Write(bar.Time + " " + bar.Close.ToString());
    }

    [Fact]
    public async Task HistoryObservableErrorTest()
    {
        Contract contract = new()
        {
            SecurityType = ContractSecurityType.Stock,
            Symbol = "InvalidSymbol",
            Currency = "USD",
            Exchange = "SMART"
        };

        await Assert.ThrowsAsync<AlertException>(async () => await Client
            .Service
            .CreateHistoricalDataObservable(contract)
            .ThrowAlertMessage());
    }

    [Fact]
    public async Task HistorySnapshotAsyncTest()
    {
        Contract contract = new()
        {
            SecurityType = ContractSecurityType.Stock,
            Symbol = "V",
            Currency = "USD",
            Exchange = "SMART"
        };

        HistoricalData data = await Client
            .Service
            .GetHistoricalDataAsync(
                contract,
                "",
                HistoricalDataDuration.OneMonth,
                HistoricalDataBarSize.OneDay,
                HistoricalDataWhatToShow.Trades);

        foreach (var bar in data.Bars)
            Write(bar.Stringify());
    }

    [Fact]
    public async Task HistorySnapshotAsyncErrorTest()
    {
        Contract contract = new()
        {
            SecurityType = ContractSecurityType.Stock,
            Symbol = "InvalidSymbol",
            Currency = "USD",
            Exchange = "SMART"
        };

        await Assert.ThrowsAsync<AlertException>(async () => await Client
            .Service
            .GetHistoricalDataAsync(contract));
    }
}
