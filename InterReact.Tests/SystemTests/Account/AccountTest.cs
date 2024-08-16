using Stringification;
using System.Reactive.Linq;

namespace Account;

public class Account(ITestOutputHelper output, TestFixture fixture) : CollectionTestBase(output, fixture)
{
    [Fact]
    public async Task AccountUpdatesTest()
    {
        IList<object> messages = await Client
            .Service
            .GetAccountUpdatesAsync();

        foreach (var m in messages)
            Write(m.Stringify());
    }

    [Fact]
    public async Task AccountUpdatesMultiTest()
    {
        IList<AccountUpdateMulti> messages = await Client
            .Service
            .GetAccountUpdatesMultiAsync();

        foreach (var m in messages)
            Write(m.Stringify());
    }

    [Fact]
    public async Task AccountSummaryTest()
    {
        IList<AccountSummary> messages = await Client
            .Service
            .GetAccountSummaryAsync();

        foreach (var m in messages)
            Write(m.Stringify());
    }

    [Fact]
    public async Task AccountPositionsMultiTest()
    {
        IList<AccountPositionsMulti> positions = await Client
            .Service
            .GetAccountPositionsMultiAsync();

        // The account may or may not have positions.
        if (positions.Count == 0)
            Write("No positions");
        foreach (var p in positions)
            Write(p.Stringify());
    }
}
