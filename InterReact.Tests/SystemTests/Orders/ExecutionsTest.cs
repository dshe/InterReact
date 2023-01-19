using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;

namespace Orders;

public class Executions : TestCollectionBase
{
    public Executions(ITestOutputHelper output, TestFixture fixture) : base(output, fixture) { }

    [Fact]
    public async Task RequestExecutionsTest()
    {
        var requestId = Client.Request.GetNextId();

        var task = Client.Response
            .OfType<ExecutionEnd>()
            .Where(x => x.RequestId == requestId)
            .FirstAsync()
            .Timeout(TimeSpan.FromSeconds(3))
            .ToTask();

        Client.Request.RequestExecutions(requestId);

        await task;
    }
}
