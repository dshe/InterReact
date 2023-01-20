using Stringification;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;

namespace Account;

public class Summary : TestCollectionBase
{
    public Summary(ITestOutputHelper output, TestFixture fixture) : base(output, fixture) { }

    [Fact]
    public async Task AccountSummaryTest()
    {
        int requestId = Client.Request.GetNextId();

        Task<IList<IHasRequestId>> task = Client
            .Response
            .OfType<IHasRequestId>()
            .Where(t => t.RequestId == requestId)
            .TakeWhile(o => o is not AccountSummaryEnd)
            .ToList()
            .ToTask();

        Client.Request.RequestAccountSummary(requestId);

        IList<IHasRequestId> list = await task; 

        Client.Request.CancelAccountSummary(requestId);

        Assert.NotEmpty(list);

        foreach (object o in list)
            Write(o.Stringify());
    }

    [Fact]
    public async Task AccountSummaryObservableTest()
    {
        IList<IHasRequestId> list = await Client
            .Service
            .AccountSummaryObservable
            .TakeWhile(o => o is not AccountSummaryEnd)
            .ToList();

        Assert.NotEmpty(list);

        foreach (object o in list)
            Write(o.Stringify());
    }
}
