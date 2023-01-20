using Stringification;

namespace Orders;

public class Open : TestCollectionBase
{
    public Open(ITestOutputHelper output, TestFixture fixture) : base(output, fixture) { }

    [Fact]
    public async Task OpenOrdersAsyncTest()
    {
        await Task.Delay(3000);

        IList<object> list = await Client
            .Service
            .GetOpenOrdersAsync(OpenOrdersRequestType.OpenOrders)
            .WaitAsync(TimeSpan.FromSeconds(6));

        Write($"Open orders found: {list.Count}.");

        foreach (var item in list)
            Write(item.Stringify());
    }
}

