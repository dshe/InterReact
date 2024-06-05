using Stringification;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;

namespace Account;

public class Summary : CollectionTestBase
{
    public Summary(ITestOutputHelper output, TestFixture fixture) : base(output, fixture) { }

    [Fact]
    public async Task AccountSummaryTest()
    {
        int id = Client.Request.GetNextId();

        Task<IList<IHasRequestId>> task = Client
            .Response
            .WithRequestId(id)
            .Take(TimeSpan.FromMilliseconds(1000))
            .ToList()
            .ToTask();

        Client.Request.RequestAccountSummary(id);

        IList<IHasRequestId> messages = await task;

        Client.Request.CancelAccountSummary(id);

        Assert.NotEmpty(messages);

        foreach (var m in messages)
            Write(m.Stringify());
    }

    [Fact]
    public async Task AccountSummaryObservableTest()
    {
        IList<AccountSummary> messages = await Client
            .Service
            .AccountSummaryObservable
            .Take(TimeSpan.FromMilliseconds(1000))
            .ToList();

        Assert.NotEmpty(messages);

        foreach (var m in messages)
            Write(m.Stringify());
    }

    [Fact]
    public async Task AccountSummaryObservableTest2()
    {
        IList<AccountSummary> messages = await Client
            .Service
            .AccountSummaryObservable
            .Take(TimeSpan.FromMilliseconds(1000))
            .ToList();

        Assert.NotEmpty(messages);

        foreach (var m in messages)
            Write(m.Stringify());
    }


}
