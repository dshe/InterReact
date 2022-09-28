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
            .Services
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
        List<object> list1 = new();
        List<object> list2 = new();

        IDisposable subscription1 = Client
            .Services
            .AccountUpdatesObservable
            .Subscribe(list1.Add);

        await Task.Delay(2000);

        IDisposable subscription2 = Client
            .Services
            .AccountUpdatesObservable
            .Subscribe(list2.Add);

        Assert.Equal(list1.Count, list2.Count); // the first list has two UpdateAccountTime objects

        subscription1.Dispose();
        subscription2.Dispose();
    }
}
