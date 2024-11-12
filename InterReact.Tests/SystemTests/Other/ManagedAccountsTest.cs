using Stringification;
using System.Reactive.Linq;
namespace ManagedAccounts;

public class ManagedAccounts(ITestOutputHelper output, TestFixture fixture) : CollectionTestBase(output, fixture)
{
    [Fact]
    public async Task ManagedAccountsObservavbleTest()
    {
        string[] accounts = await Client
            .Service
            .CreateManagedAccountsObservable()
            .Timeout(TimeSpan.FromSeconds(3));

        foreach (string account in accounts)
            Write(account.Stringify());
    }

    [Fact]
    public async Task ManagedAccountsAsyncTest()
    {
        string[] accounts = await Client
            .Service
            .GetManagedAccountsAsync(TimeSpan.FromSeconds(3));

        foreach (string account in accounts)
            Write(account.Stringify());
    }


}
