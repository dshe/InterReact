using Stringification;

namespace ManagedAccounts;

public class ManagedAccounts(ITestOutputHelper output, TestFixture fixture) : CollectionTestBase(output, fixture)
{
    [Fact]
    public async Task ManagedAccountsTest()
    {
        IList<string> accounts = await Client
            .Service
            .GetManagedAccountsAsync();

        foreach (var m in accounts)
            Write(m.Stringify());
    }
}
