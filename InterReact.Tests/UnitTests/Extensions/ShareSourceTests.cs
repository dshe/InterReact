using Microsoft.Reactive.Testing;
using Stringification;
using System.Reactive;
using System.Reactive.Linq;

namespace Extension;

public sealed class ShareSourceTests(ITestOutputHelper output) : ReactiveUnitTestBase(output)
{
    private readonly TestScheduler TestScheduler = new();

    [Fact]
    public void T01_Consecutive()
    {
        ITestableObserver<string> observer1 = TestScheduler.CreateObserver<string>();
        ITestableObserver<string> observer2 = TestScheduler.CreateObserver<string>();

        ITestableObservable<string> observable = TestScheduler.CreateColdObservable(
            OnNext(100, "1"),
            OnCompleted<string>(500)
        );

        IObservable<string> sharedObservable = observable.ShareSource();

        sharedObservable.SubscribeOn(TestScheduler).Subscribe(observer1);

        TestScheduler.AdvanceBy(1000);

        sharedObservable.SubscribeOn(TestScheduler).Subscribe(observer2);

        TestScheduler.Start();

        Recorded<Notification<string>>[] expected = new[]
        {
                OnNext(101, "1"),
                OnCompleted<string>(501)
            };
        Assert.Equal(expected.ToList(), observer1.Messages);

        expected = new[]
        {
                OnNext(1101, "1"),
                OnCompleted<string>(1501)
            };
        Assert.Equal(expected.ToList(), observer2.Messages);

        Assert.Equal(2, observable.Subscriptions.Count);
        Write(observable.Subscriptions.Stringify());
    }

    [Fact]
    public void T02_Overlapping()
    {
        ITestableObserver<string> observer1 = TestScheduler.CreateObserver<string>();
        ITestableObserver<string> observer2 = TestScheduler.CreateObserver<string>();

        ITestableObservable<string> observable = TestScheduler.CreateColdObservable(
            OnNext(100, "1"),
            OnNext(200, "2"),
            OnNext(300, "3"),
            OnNext(400, "4"),
            OnNext(500, "5"),
            OnCompleted<string>(600));

        IObservable<string> sharedObservable = observable.ShareSource();

        sharedObservable.SubscribeOn(TestScheduler).Subscribe(observer1);

        TestScheduler.AdvanceBy(350);

        sharedObservable.SubscribeOn(TestScheduler).Subscribe(observer2);

        TestScheduler.Start();

        Recorded<Notification<string>>[] expected = new[]
        {
                OnNext(101, "1"),
                OnNext(201, "2"),
                OnNext(301, "3"),
                OnNext(401, "4"),
                OnNext(501, "5"),
                OnCompleted<string>(601)
            };
        Assert.Equal(expected.ToList(), observer1.Messages);

        expected = new[]
        {
                OnNext(351, "1"),
                OnNext(351, "2"),
                OnNext(351, "3"),
                OnNext(401, "4"),
                OnNext(501, "5"),
                OnCompleted<string>(601)
            };
        Assert.Equal(expected.ToList(), observer2.Messages);
        Assert.Equal(1, observable.Subscriptions.Count);
    }

    [Fact]
    public void T03_Concurrent()
    {
        ITestableObserver<string> observer1 = TestScheduler.CreateObserver<string>();
        ITestableObserver<string> observer2 = TestScheduler.CreateObserver<string>();

        ITestableObservable<string> observable = TestScheduler.CreateColdObservable(
            OnNext(100, "1"),
            OnNext(200, "2"),
            OnNext(300, "3"),
            OnNext(400, "4"),
            OnNext(500, "5"),
            OnCompleted<string>(600));

        IObservable<string> sharedObservable = observable.ShareSource();

        sharedObservable.SubscribeOn(TestScheduler).Subscribe(observer1);
        sharedObservable.SubscribeOn(TestScheduler).Subscribe(observer2);

        TestScheduler.Start();

        Recorded<Notification<string>>[] expected = new[]
        {
                OnNext(101, "1"),
                OnNext(201, "2"),
                OnNext(301, "3"),
                OnNext(401, "4"),
                OnNext(501, "5"),
                OnCompleted<string>(601)
            };

        Assert.Equal(expected.ToList(), observer1.Messages);
        Assert.Equal(expected.ToList(), observer2.Messages);
        Assert.Equal(1, observable.Subscriptions.Count);
    }
}
