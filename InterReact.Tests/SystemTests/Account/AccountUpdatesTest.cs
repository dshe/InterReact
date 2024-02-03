using Stringification;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;

namespace Account;

public class Updates : CollectionTestBase
{
    public Updates(ITestOutputHelper output, TestFixture fixture) : base(output, fixture) { }

    [Fact(Skip = "May interfere with AccountUpdatesObservableTest")]
    public async Task AccountUpdatesTest()
    {
        IList<string> accounts = await Client
            .Service
            .ManagedAccountsObservable
            .Timeout(TimeSpan.FromSeconds(1));

        string account = accounts.Last();

        Task<IList<object>> task = Client
            .Response
            .Where(m => m is AccountValue or PortfolioValue or AccountUpdateTime or AccountUpdateEnd)
            .Take(TimeSpan.FromMilliseconds(1000))
            .ToList()
            .ToTask();

        Client.Request.RequestAccountUpdates(true, account);

        IList<object> messages = await task;

        Client.Request.RequestAccountUpdates(false, account);

        foreach (var m in messages)
            Write(m.Stringify());
    }

    [Fact]
    public async Task AccountUpdatesObservableTest()
    {
        IList<string> accounts = await Client
            .Service
            .ManagedAccountsObservable
            .Timeout(TimeSpan.FromSeconds(1));

        string account = accounts.Last();

        IList<object> messages = await Client
            .Service
            .CreateAccountUpdatesObservable(account)
            .Take(TimeSpan.FromMilliseconds(100))
            .ToList();

        foreach (var m in messages)
            Write(m.Stringify());
    }
}
