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
        int id = Client.Request.GetNextId();

        Task<IList<object>> task = Client
            .Response
            .WithRequestId(id)
            .TakeWhile(o => o is not AccountSummaryEnd)
            .ToList()
            .ToTask();

        Client.Request.RequestAccountSummary(id);

        IList<object> list = await task; 

        Client.Request.CancelAccountSummary(id);

        Assert.NotEmpty(list);

        foreach (object o in list)
            Write(o.Stringify());
    }

    [Fact]
    public async Task AccountSummaryObservableTest()
    {
        IList<object> list = await Client
            .Service
            .AccountSummaryObservable
            .TakeWhile(o => o is not AccountSummaryEnd)
            .ToList();

        Assert.NotEmpty(list);

        foreach (object o in list)
            Write(o.Stringify());
    }
}
