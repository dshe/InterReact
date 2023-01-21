using System.Diagnostics;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Utility;

public class ToObservableContinuous : UnitTestBase
{
    public ToObservableContinuous(ITestOutputHelper output) : base(output) { }

    private int subscribeCalls, unsubscribeCalls;
    private readonly Subject<object> subject = new();

    public class SomeClass
    {
        public long Time { get; } = Stopwatch.GetTimestamp();
    }

    [Theory,
    InlineData(-1),
    InlineData(0),
    InlineData(1),
    InlineData(10),
    InlineData(100)]
    public async Task TimeoutTest(int ticks)
    {
        TimeSpan ts = ticks == -1 ? TimeSpan.Zero : TimeSpan.FromTicks(ticks);
        IObservable<object> observable = subject.ToObservableContinuous(() => Interlocked.Increment(ref subscribeCalls), () => Interlocked.Increment(ref unsubscribeCalls));
        await Assert.ThrowsAsync<TimeoutException>(async () => await observable.Timeout(ts));
        await Task.Delay(1);
        Assert.Equal(1, subscribeCalls);
        Assert.Equal(1, unsubscribeCalls);
    }
}
