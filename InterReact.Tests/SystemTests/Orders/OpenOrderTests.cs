using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;

namespace Orders;

public class OpenOrders : TestCollectionBase
{
    public OpenOrders(ITestOutputHelper output, TestFixture fixture) : base(output, fixture) { }

    [Fact]
    public async Task OpenOrdersTest()
    {
        var task = Client.Response
            .OfType<OpenOrderEnd>()
            .FirstAsync()
            .Timeout(TimeSpan.FromSeconds(3))
            .ToTask();

        Client.Request.RequestOpenOrders();

        await task;
    }
}

