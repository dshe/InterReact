using Stringification;
using System.Reactive.Linq;
namespace Account;

public class AccountUpdate(ITestOutputHelper output, TestFixture fixture) : CollectionTestBase(output, fixture)
{
    [Fact]
    public async Task AccountUpdatesMultiObservableSubscription()
    {
        // This observable does not complete until subscription.Dispose().
        IObservable<AccountUpdateMulti> observable = Client.Service.CreateAccountUpdatesMultiObservable();

        IDisposable subscription = observable.Subscribe(
            onNext: m => Write(m.Stringify()),
            onError: ex => Write(ex.Message),
            onCompleted: () => Write("Completed")); 

        await Task.Delay(TimeSpan.FromSeconds(1));

        subscription.Dispose();
    }

    [Fact]
    public async Task AccountUpdatesMultiObservableTest()
    {
        AccountUpdateMulti[] messages = await Client
            .Service
            .CreateAccountUpdatesMultiObservable()
            .TakeUntil(m => m.IsEndMessage)
            .ToArray()
            .Timeout(TimeSpan.FromSeconds(2));

        Assert.True(messages.Length > 1);
        Assert.NotNull(messages.Where(m => m.IsEndMessage).SingleOrDefault());

        foreach (var m in messages)
            Write(m.Stringify());
    }

    [Fact]
    public async Task AccountUpdatesMultiObservableCache()
    {
        // This observable does not complete until subscription.Dispose().
        IObservable<AccountUpdateMulti> observable = Client.Service.CreateAccountUpdatesMultiObservable();

        List<AccountUpdateMulti> messages1 = [], messages2 = [];

        IDisposable subscription1 = observable.Subscribe(messages1.Add);
        await Task.Delay(TimeSpan.FromMilliseconds(200));
        IDisposable subscription2 = observable.Subscribe(messages2.Add);

        await Task.Delay(TimeSpan.FromSeconds(2));

        subscription1.Dispose();
        subscription2.Dispose();

        Assert.True(messages1.Count > 100);
        Assert.True(messages2.Count > 100);
    }

}
