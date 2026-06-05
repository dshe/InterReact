using System.Reactive.Linq;
namespace SystemTests;

public class AccountPositions(ITestOutputHelper output, TestFixture fixture) : CollectionTestBase(output, fixture)
{
    [Fact]
    public async Task AccountPositionsObservableTestAsync()
    {

        AccountPosition[] messages = await Client
            .Service
            .AccountPositionsObservable
            .TakeWhile(a => !a.IsEndMessage)
            .Timeout(TimeSpan.FromSeconds(3))
            .ToArray();

        if (messages.Length == 0)
            Write("no positions");
        foreach (var m in messages)
            Write(m.Stringify());
    }

    [Fact]
    public async Task AccountPositionsAsyncTestAsync()
    {
        AccountPosition[] messages = await Client
            .Service
            .GetAccountPositionsAsync(timeout: TimeSpan.FromSeconds(3), TestContext.Current.CancellationToken);

        if (messages.Length == 0)
            Write("no positions");
        foreach (var m in messages)
            Write(m.Stringify());
    }

}
