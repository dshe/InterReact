using Stringification;

namespace Orders;

public class Completed : CollectionTestBase
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

        foreach (CompletedOrder order in orders)
            Write(order.Stringify());
    }
}
