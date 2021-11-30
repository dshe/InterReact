using System;
using System.Diagnostics;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace InterReact.UnitTests.Utility;

public class ToObservableContinuousTest : UnitTestsBase
{
    public ToObservableContinuousTest(ITestOutputHelper output) : base(output) { }

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
    public async Task Test_Timeout(int ticks)
    {
        var ts = ticks == -1 ? TimeSpan.Zero : TimeSpan.FromTicks(ticks);
        var observable = subject.ToObservableContinuous(() => Interlocked.Increment(ref subscribeCalls), () => Interlocked.Increment(ref unsubscribeCalls));
        await Assert.ThrowsAsync<TimeoutException>(async () => await observable.Timeout(ts));
        await Task.Delay(1);
        Assert.Equal(1, subscribeCalls);
        Assert.Equal(1, unsubscribeCalls);
    }
}
