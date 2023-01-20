using Stringification;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;

namespace Orders;

public class Completed : TestCollectionBase
{
    public Completed(ITestOutputHelper output, TestFixture fixture) : base(output, fixture) { }

    [Fact]
    public async Task CompletedOrdersAsyncTest()
    {
        bool api = true;

        IList<CompletedOrder> orders = await Client
            .Service
            .GetCompleteOrdersAsync(api);

        Write($"Complete orders found: {orders.Count}.");

        foreach (var order in orders)
            Write(order.Stringify());
    }
}
