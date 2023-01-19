﻿using System.Diagnostics;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Utility;

public class ToObservableSingle : UnitTestBase
{
    public ToObservableSingle(ITestOutputHelper output) : base(output) { }

    private int subscribeCalls;
    private readonly Subject<object> subject = new();

    public class SomeClass
    {
        public long Time { get; } = Stopwatch.GetTimestamp();
    }

    public class SomeClassEnd { }


    [Fact]
    public async Task OkTest()
    {
        var observable = subject.ToObservableSingle<SomeClass>(
            () =>
            {
                Interlocked.Increment(ref subscribeCalls);
                subject.OnNext(new SomeClass());
            });

        var n1 = await observable.Materialize().ToList();

        Assert.Equal(2, n1.Count);
        Assert.Equal(NotificationKind.OnNext, n1[0].Kind);
        Assert.Equal(NotificationKind.OnCompleted, n1[1].Kind);

        var n2 = await observable.Materialize().ToList();
        Assert.Equal(2, n2.Count);
        Assert.Equal(NotificationKind.OnNext, n2[0].Kind);
        Assert.Equal(NotificationKind.OnCompleted, n2[1].Kind);

        Assert.NotEqual(n1[0].Value.Time, n2[0].Value.Time);
        Assert.Equal(2, subscribeCalls);
    }

    [Fact]
    public async Task SubscribeErrorTest()
    {
        var observable = subject.ToObservableSingle<string>(() => { throw new BarrierPostPhaseException(); });
        await Assert.ThrowsAsync<BarrierPostPhaseException>(async () => await observable);
    }

    [Fact]
    public async Task SourceErrorTest()
    {
        var observable = subject.ToObservableSingle<string>(() => subject.OnError(new BarrierPostPhaseException()));
        await Assert.ThrowsAsync<BarrierPostPhaseException>(async () => await observable);
    }

    [Fact]
    public async Task SourceCompletedTest()
    {
        var observable = subject.ToObservableSingle<string>(() => subject.OnCompleted());
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () => await observable);
        Assert.Equal("Sequence contains no elements.", ex.Message);
    }
}
