using Stringification;

namespace Orders;

public class Executions : TestCollectionBase
{
    public Executions(ITestOutputHelper output, TestFixture fixture) : base(output, fixture) { }

    [Fact]
    public async Task RequestExecutionsAsyncTest()
    {
        IList<object> list = await Client
            .Service
            .GetExecutionsAsync();
           
        Write($"Executions found: {list.Count}.");

        foreach (IHasRequestId item in list)
            Write(item.Stringify());
    }
}
