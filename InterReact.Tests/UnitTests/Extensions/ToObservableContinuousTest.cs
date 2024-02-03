using Microsoft.Reactive.Testing;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Extension;

public class ToObservableContinuous : ReactiveUnitTestBase
{
    private readonly TestScheduler TestScheduler = new();

    public ToObservableContinuous(ITestOutputHelper output) : base(output) { }

    [Theory,
    InlineData(-1),
    InlineData(0),
    InlineData(1),
    InlineData(10),
    InlineData(100)]
    public async Task TimeoutTest(int ticks)
    {
        int start = 0, stop = 0;

        TimeSpan ts = ticks == -1 ? TimeSpan.Zero : TimeSpan.FromTicks(ticks);

        Subject<object> subject = new();

        IObservable<object> observable = subject.ToObservableContinuous(() => Interlocked.Increment(ref start), () => Interlocked.Increment(ref stop));

        await Assert.ThrowsAsync<TimeoutException>(async () => await observable.Timeout(ts));

        Assert.Equal(1, start);
        Assert.Equal(1, stop);
    }

    [Fact]
    public void OnNextTest()
    {
        int start = 0, stop = 0;

        ITestableObservable<int> source =
            TestScheduler.CreateHotObservable<int>(
                    OnNext(10, 1),
                    OnNext(20, 2));

        ITestableObserver<int> observer = TestScheduler.CreateObserver<int>();

        IDisposable subscription = source.ToObservableContinuous(() => start++, () => stop++)
           .Subscribe(observer);

        TestScheduler.Start();
        subscription.Dispose();

        observer.Messages.AssertEqual(
                    OnNext(10, 1),
                    OnNext(20, 2));

        Assert.Equal(1, start);
        Assert.Equal(1, stop);
    }
}
