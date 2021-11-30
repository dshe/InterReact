using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace InterReact.SystemTests.MarketData;

public class MarketDataSnapshotServiceTests : TestCollectionBase
{
    public MarketDataSnapshotServiceTests(ITestOutputHelper output, TestFixture fixture) : base(output, fixture) { }

    [Fact]
    public async Task TestTickSnapshot()
    {
        Contract contract = new()
        { SecurityType = SecurityType.Stock, Symbol = "IBM", Currency = "USD", Exchange = "SMART" };

        Client.Request.RequestMarketDataType(MarketDataType.Delayed);

        IList<ITick> ticks = await Client
            .Services
            .CreateTickSnapshotObservable(contract)
            .ToList();

        Assert.NotEmpty(ticks);
    }

    [Fact]
    public async Task TestTickSnapshotInvalid()
    {
        Contract contract = new()
        { SecurityType = SecurityType.Stock, Symbol = "InvalidSymbol", Currency = "USD", Exchange = "SMART" };

        Client.Request.RequestMarketDataType(MarketDataType.Delayed);

        var alertException = await Assert.ThrowsAsync<AlertException>(async () => await Client
                .Services
                .CreateTickSnapshotObservable(contract)
                .ToList());

        Write(alertException.Message);
    }
}
