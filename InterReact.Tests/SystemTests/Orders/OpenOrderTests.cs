using Stringification;

namespace Orders;

public class Open : CollectionTestBase
{
    public Open(ITestOutputHelper output, TestFixture fixture) : base(output, fixture) { }

    [Fact(Skip = "Test may conflict when run together with order placement tests")]
    public async Task OpenOrdersAsyncTest()
    {
        await Task.Delay(3000);

        IList<Object> list = await Client
            .Service
            .RequestOpenOrdersAsync()
            .WaitAsync(TimeSpan.FromSeconds(10));

        Write($"Open orders found: {list.Count}.");

        foreach (Object item in list)
            Write(item.Stringify());
    }
}

