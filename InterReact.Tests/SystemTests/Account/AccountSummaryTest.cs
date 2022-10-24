using Stringification;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace InterReact.SystemTests.Account;

public class AccountSummaryTest : TestCollectionBase
{
    public AccountSummaryTest(ITestOutputHelper output, TestFixture fixture) : base(output, fixture) { }

    [Fact]
    public async Task AccountSummary()
    {
        IList<IHasRequestId> list = await Client
            .Service
            .AccountSummaryObservable
            .TakeWhile(o => o is not AccountSummaryEnd)
            .ToList();

        foreach (object o in list)
            Write(o.Stringify());
    }
}
