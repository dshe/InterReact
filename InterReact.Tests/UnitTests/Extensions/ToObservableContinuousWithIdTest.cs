using Microsoft.Reactive.Testing;
using System.Diagnostics;

namespace Extension;

public class ToObservableContinuousWithId : ReactiveUnitTestBase
{
    public ToObservableContinuousWithId(ITestOutputHelper output) : base(output) { }

    public interface ISomeClass : IHasRequestId { }
    public class SomeClass : ISomeClass
    {
        public SomeClass(int id) {  RequestId= id; }
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
        var now = SystemClock.Instance.GetCurrentInstant();

        var o1 = new SomeClass(42);
        var o2 = new SomeClass(100);
        var o3 = new Alert { Id = 42, Time = now };
        var o4 = new Alert { Id = 42, Time = now };
        var o5 = new SomeClass(42);
        var o6 = new SomeClass(99);

        ITestableObservable<object> source =
            testScheduler.CreateHotObservable(
                    OnNext<object>(10, o1),
                    OnNext<object>(20, o2),
                    OnNext<object>(30, o3),
                    OnNext<object>(40, o4),
                    OnNext<object>(50, o5),
                    OnNext<object>(60, o6));

        ITestableObserver<object> observer = testScheduler.CreateObserver<object>();

        var subscription = source.ToObservableContinuousWithId(() => 42, id => start++, id => stop++)
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
