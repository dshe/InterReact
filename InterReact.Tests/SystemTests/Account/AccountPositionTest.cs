using Stringification;
using System.Reactive.Linq;
namespace Account;

public class AccountPosition(ITestOutputHelper output, TestFixture fixture) : CollectionTestBase(output, fixture)
{
    [Fact]
    public async Task AccountPositionsMultiObservableTest()
    {
        AccountPositionsMulti[] messages = await Client
            .Service
            .CreateAccountPositionsMultiObservable()
            .TakeWhile(m => !m.IsEndMessage)
            .ToArray()
            .Timeout(TimeSpan.FromSeconds(1));

        if (messages.Length == 0)
            Write("no positions");
        foreach (var m in messages)
            Write(m.Stringify());
    }

}
