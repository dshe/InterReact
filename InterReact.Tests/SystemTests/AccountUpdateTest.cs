using Stringification;
using System.Reactive.Linq;
namespace SystemTests;

public class AccountUpdate(ITestOutputHelper output, TestFixture fixture) : CollectionTestBase(output, fixture)
{
    [Fact]
    public async Task RequestAccountUpdatesMultiTest()
    {
        IReadOnlyList<string> accounts = await Client.Service.GetManagedAccountsAsync(TimeSpan.FromSeconds(3), TestContext.Current.CancellationToken);

        IDisposable subscription = Client.Response.Subscribe(m => Write(m.Stringify()));

        await Client.Request.RequestAccountUpdatesMulti(119, "All");
        //Client.Request.RequestAccountSummary(123);

        await Task.Delay(TimeSpan.FromSeconds(3), TestContext.Current.CancellationToken);

        await Client.Request.CancelAccountUpdatesMulti(119);
        //Client.Request.CancelAccountSummary(123);
        subscription.Dispose();
    }

    [Fact]
    public async Task AccountUpdatesMultiObservableTest()
    {
        IReadOnlyList<string> accounts = await Client.Service.GetManagedAccountsAsync(TimeSpan.FromSeconds(3), TestContext.Current.CancellationToken);

        AccountUpdateMulti[] messages = await Client
            .Service
            .CreateAccountUpdatesMultiObservable(accounts[0])
            .OfTypeOnly<AccountUpdateMulti>()
            .TakeUntil(m => m.IsEndMessage)
            .Timeout(TimeSpan.FromSeconds(2))
            .ToArray();

        Assert.True(messages.Length > 1);
        Assert.NotNull(messages.Where(m => m.IsEndMessage).SingleOrDefault());

        foreach (var m in messages)
            Write(m.Stringify());
    }

    [Fact]
    public async Task AccountUpdatesMultiObservableCache()
    {
        IReadOnlyList<string> accounts = await Client.Service.GetManagedAccountsAsync(TimeSpan.FromSeconds(3), TestContext.Current.CancellationToken);

        // This observable does not complete until subscription.Dispose().
        IObservable<AccountUpdateMulti> observable = Client
            .Service
            .CreateAccountUpdatesMultiObservable(accounts[0])
            .OfTypeOnly<AccountUpdateMulti>();

        List<AccountUpdateMulti> messages1 = [], messages2 = [], messages3 = [];

        IDisposable subscription1 = observable.Subscribe(messages1.Add);
        await Task.Delay(TimeSpan.FromMilliseconds(10), TestContext.Current.CancellationToken);
        IDisposable subscription2 = observable.Subscribe(messages2.Add);
        await Task.Delay(TimeSpan.FromMilliseconds(10), TestContext.Current.CancellationToken);
        IDisposable subscription3 = observable.Subscribe(messages3.Add);

        await Task.Delay(TimeSpan.FromSeconds(1), TestContext.Current.CancellationToken);

        subscription1.Dispose();
        subscription2.Dispose();
        subscription3.Dispose();

        Assert.True(messages1.Count > 100); // 186 (AccruedCash, USD, was output twice)
        Assert.True(messages2.Count > 100); // 185
        Assert.True(messages3.Count > 100); // 185

        var xx = messages1.Except(messages2).ToList();
        var xy = messages2.Except(messages2).ToList();
        ;
    }

    [Fact]
    public async Task AccountUpdatesMultiAsync()
    {
        IReadOnlyList<string> accounts = await Client.Service.GetManagedAccountsAsync(TimeSpan.FromSeconds(3), TestContext.Current.CancellationToken);

        IHasRequestId[] messages = await Client
            .Service
            .GetAccountUpdatesMultiAsync(accounts[0], "", false, TimeSpan.FromSeconds(3), TestContext.Current.CancellationToken);

        Assert.True(messages.Length > 100);
    }


}
