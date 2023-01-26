using Microsoft.Reactive.Testing;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Extension;

public class ToObservableContinuous : ReactiveUnitTestBase
{
    public ToObservableContinuous(ITestOutputHelper output) : base(output) { }

    [Theory,
    InlineData(-1),
    InlineData(0),
    InlineData(1),
    InlineData(10),
    InlineData(100)]
    public async Task TimeoutTest(int ticks)
    {
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
        ITestableObservable<int> source =
            testScheduler.CreateHotObservable<int>(
                    OnNext(10, 1),
                    OnNext(20, 2));

        ITestableObserver<int> observer = testScheduler.CreateObserver<int>();

       var subscription = source.ToObservableContinuous(() => start++, () => stop++)
           .Subscribe(observer);

        testScheduler.Start();
        subscription.Dispose();

        observer.Messages.AssertEqual(
                    OnNext(10, 1),
                    OnNext(20, 2));

        Assert.Equal(1, start);
        Assert.Equal(1, stop);
    }
}
