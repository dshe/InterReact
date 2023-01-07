using Stringification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace InterReact.SystemTests.Account;

public class AccountUpdateTest : TestCollectionBase
{
    public AccountUpdateTest(ITestOutputHelper output, TestFixture fixture) : base(output, fixture) { }

    [Fact]
    public async Task TakeTest()
    {
        IList<object> list = await Client
            .Service
            .AccountUpdatesObservable
            .TakeWhile(o => o is not AccountUpdateEnd)
            //.Take(TimeSpan.FromSeconds(2))
            .ToList();

        foreach (object o in list)
            Write(o.Stringify());
    }

    [Fact]
    public async Task Multi_Subscriber_Test()
    {
        Stringifier stringifier = new();

        HashSet<string> list1 = new();
        HashSet<string> list2 = new();

        IDisposable subscription1 = Client
            .Service
            .AccountUpdatesObservable
            .Select(x => x.Stringify())
            .Subscribe(x => list1.Add(x));

        await Task.Delay(2000);

        IDisposable subscription2 = Client
            .Service
            .AccountUpdatesObservable
            .Select(x => x.Stringify())
            .Subscribe(x => list2.Add(x));

        List<string> diff = list1.Except(list2).ToList();
        Assert.Equal(1, diff.Count);
        // the first list has two UpdateAccountTime objects

        subscription1.Dispose();
        subscription2.Dispose();
    }
}
