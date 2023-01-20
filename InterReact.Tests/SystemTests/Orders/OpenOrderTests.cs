using Stringification;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;

namespace Orders;

public class Open : TestCollectionBase
{
    public Open(ITestOutputHelper output, TestFixture fixture) : base(output, fixture) { }

    [Fact]
    public async Task OpenOrdersAsyncTest()
    {
        IList<object> list = await Client
            .Service
            .GetOpenOrdersAsync(OpenOrdersRequestType.OpenOrders);

        Write($"Open orders found: {list.Count}.");

        foreach (var item in list)
            Write(item.Stringify());
    }
}

