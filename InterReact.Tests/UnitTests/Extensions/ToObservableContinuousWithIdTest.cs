using Microsoft.Reactive.Testing;
using System.Diagnostics;

namespace Extension;

public class ToObservableContinuousWithId : ReactiveUnitTestBase
{
    public ToObservableContinuousWithId(ITestOutputHelper output) : base(output) { }

    public interface ISomeClass : IHasRequestId { }
    public class SomeClass : ISomeClass
    {
        public SomeClass(int id) { RequestId= id; }
        public int RequestId { get; }
        public long Ticks { get; } = Stopwatch.GetTimestamp();
    }
    public class SomeClassEnd : ISomeClass
    {
        public SomeClassEnd(int id) { RequestId = id; }
        public int RequestId { get; }
    }

    [Fact]
    public void OnNextTest()
    {
        TestScheduler testScheduler = new();

        int start = 0, stop = 0;

        Instant now = SystemClock.Instance.GetCurrentInstant();

        SomeClass o1 = new(42);
        SomeClass o2 = new(100);
        AlertMessage o3 = new() { RequestId = 42, Time = now };
        AlertMessage o4 = new() { RequestId = 42, Time = now };
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

        IDisposable subscription = source.ToObservableContinuousWithId(() => 42, id => start++, id => stop++)
            .Subscribe(observer);

        testScheduler.Start();
        subscription.Dispose();

        observer.Messages.AssertEqual(
                    OnNext<object>(10, o1),
                    OnNext<object>(30, o3),
                    OnNext<object>(40, o4),
                    OnNext<object>(50, o5));

        Assert.Equal(1, start);
        Assert.Equal(1, stop);
    }
   
}
