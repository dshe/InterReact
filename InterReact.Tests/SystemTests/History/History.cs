using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;

namespace History;

public class HistoryTests : TestCollectionBase
{
    public HistoryTests(ITestOutputHelper output, TestFixture fixture) : base(output, fixture) { }

    [Fact]
    public async Task TestHistory()
    {
        Contract contract = new()
        {
            SecurityType = SecurityType.Stock,
            Symbol = "AMZN",
            Currency = "USD",
            Exchange = "SMART"
        };

        int requestId = Client.Request.GetNextId();

        Task<IHasRequestId> task = Client
            .Response
            .WithRequestId(requestId)
            .FirstAsync()
            .ToTask();

        Client.Request.RequestHistoricalData(requestId, contract);

        IHasRequestId msg = await task;

        Assert.IsType<HistoricalData>(msg);
    }

    [Fact]
    public async Task TestHistoryWithUpdates()
    {
        Contract contract = new()
        {
            SecurityType = SecurityType.Stock,
            Symbol = "TSLA",
            Currency = "USD",
            Exchange = "SMART"
        };

        int requestId = Client.Request.GetNextId();

        Task<IHasRequestId> task = Client
            .Response
            .WithRequestId(requestId)
            .FirstAsync()
            .ToTask();

        Client.Request.RequestHistoricalData(requestId, contract);

        IHasRequestId msg = await task;

        Assert.IsType<HistoricalData>(msg);

        await Task.Delay(5000);

    }

}

