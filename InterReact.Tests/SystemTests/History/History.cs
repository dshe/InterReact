using Stringification;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;

namespace History;

public class HistoryTests : CollectionTestBase
{
    public HistoryTests(ITestOutputHelper output, TestFixture fixture) : base(output, fixture) { }

    [Fact]
    public async Task Test1History()
    {
        Contract contract = new()
        {
            SecurityType = ContractSecurityType.Stock,
            Symbol = "AMZN",
            Currency = "USD",
            Exchange = "SMART"
        };

        int id = Client.Request.GetNextId();

        Task<HistoricalData> task = Client
            .Response
            .WithRequestId(id)
            .AlertMessageToError()
            .Cast<HistoricalData>()
            .FirstAsync()
            .ToTask();

        Client.Request.RequestHistoricalData(
            id,
            contract,
            "", // up to the current date/time
            "1 M",
            "1 day",
            "TRADES");

        HistoricalData data = await task;
        Assert.NotEmpty(data.Bars);
        foreach (var bar in data.Bars)
            Write(bar.Stringify());
    }

    [Fact]
    public async Task Test2HistoryAsync()
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
                "1 M",
                "1 day",
                "TRADES");

        foreach (var bar in data.Bars)
            Write(bar.Stringify());
    }

    [Fact]
    public async Task Test3HistoryContinuousObservable()
    {
        Contract contract = new()
        {
            SecurityType = ContractSecurityType.Stock,
            Symbol = "MSFT",
            Currency = "USD",
            Exchange = "SMART"
        };

        IList<HistoricalDataBar> bars = await Client
            .Service
            .CreateHistoricalDataContinuousObservable(
                contract,
                "1 D",
                "10 secs",
                "TRADES")
            // wait some time for TWS to send some updates
            .Take(TimeSpan.FromSeconds(30))
            .ToList();

        Assert.NotEmpty(bars);
        foreach (var bar in bars)
            Write(bar.Stringify());
    }
}
