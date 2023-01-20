using Stringification;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;

namespace Account;

public class Updates : TestCollectionBase
{
    public Updates(ITestOutputHelper output, TestFixture fixture) : base(output, fixture) { }

    [Fact(Skip ="AccountUpdates may interfere with AccountUpdatesService")]
    public async Task AccountUpdatesTest()
    {
        Task<IList<object>> task = Client
            .Response
            .Where(m => m is AccountValue || m is PortfolioValue || m is AccountUpdateTime || m is AccountUpdateEnd)
            .TakeWhile(o => o is not AccountUpdateEnd)
            .ToList()
            .ToTask();

        Client.Request.RequestAccountUpdates(true);

        IList<object> list = await task;

        Client.Request.RequestAccountUpdates(false);

        foreach (object o in list)
            Write(o.Stringify());
    }

    [Fact]
    public async Task AccountUpdatesObservableTest()
    {
        IList<object> list = await Client
            .Service
            .AccountUpdatesObservable
            .TakeWhile(o => o is not AccountUpdateEnd)
            .ToList();

        foreach (object o in list)
            Write(o.Stringify());
    }
}
