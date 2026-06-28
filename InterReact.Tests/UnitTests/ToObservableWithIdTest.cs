using Microsoft.Reactive.Testing;
using System.Diagnostics;
namespace UnitTests;

public class ToObservableWithId(ITestOutputHelper output) : OutputHelperTestBase(output)
{
    public interface ISomeClass : IHasRequestId { }
    public class SomeClass(int id) : ISomeClass
    {
        public int RequestId { get; } = id; public long Ticks { get; } = Stopwatch.GetTimestamp();
    }
    public class SomeClassEnd(int id) : ISomeClass
    {
        public int RequestId { get; } = id;
    }

    [Fact]
    public void OnNextTest()
    {
        TestScheduler testScheduler = new();

        int start = 0, stop = 0;

        Instant now = SystemClock.Instance.GetCurrentInstant();

        SomeClass o1 = new(42);
        SomeClass o2 = new(100);
        Alert o3 = new() { RequestId = 42 };
        Alert o4 = new() { RequestId = 42 };
        SomeClass o5 = new(42);
        SomeClass o6 = new(99);

        ITestableObservable<object> source =
            testScheduler.CreateHotObservable(
                    OnNext<object>(10, o1),
                    OnNext<object>(20, o2),
                    OnNext<object>(30, o3),
                    OnNext<object>(40, o4),
                    OnNext<object>(50, o5),
                    OnNext<object>(60, o6));

        ITestableObserver<object> observer = testScheduler.CreateObserver<object>();

        IDisposable subscription = source
            .ToObservableWithId(() =>42, async x => start++, async x => stop++)
            .Subscribe(observer);

        testScheduler.Start();
        subscription.Dispose();

        Recorded<System.Reactive.Notification<object>> xx = OnNext<object>(10, o1);

        observer.Messages.AssertEqual(
                    OnNext<object>(10, o1),
                    OnNext<object>(30, o3),
                    OnNext<object>(40, o4),
                    OnNext<object>(50, o5));

        Assert.Equal(1, start);
        Assert.Equal(1, stop);
    }

}
