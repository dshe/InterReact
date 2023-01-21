using System.Diagnostics;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Utility;

public class ToObservableSingleWithId : UnitTestBase
{
    private const int Id = 42;
    private int subscribeCalls, unsubscribeCalls;
    private readonly Subject<object> subject = new();

    public interface ISomeClass : IHasRequestId { }
    public class SomeClass : ISomeClass
    {
        public int RequestId { get; } = Id;
        public long Ticks { get; } = Stopwatch.GetTimestamp();
    }
    public class SomeClassEnd : ISomeClass
    {
        public int RequestId { get; } = Id;
    }

    public ToObservableSingleWithId(ITestOutputHelper output) : base(output) { }

    [Fact]
    public async Task OkTest()
    {
        IObservable<SomeClass> observable = subject.ToObservableSingleWithRequestId(
            () => Id,
            requestId =>
            {
                Assert.Equal(Id, requestId);
                Interlocked.Increment(ref subscribeCalls);
                subject.OnNext(new SomeClass());
            },
            requestId =>
            {
                Assert.Equal(Id, requestId);
                Interlocked.Increment(ref unsubscribeCalls);
            })
            .Cast<SomeClass>();

        IList<Notification<SomeClass>> n1 = await observable.Materialize().ToList();
        Assert.Equal(2, n1.Count);
        Assert.Equal(NotificationKind.OnNext, n1[0].Kind);
        Assert.Equal(NotificationKind.OnCompleted, n1[1].Kind);

        IList<Notification<SomeClass>> n2 = await observable.Materialize().ToList();
        Assert.Equal(2, n2.Count);
        Assert.Equal(NotificationKind.OnNext, n2[0].Kind);
        Assert.Equal(NotificationKind.OnCompleted, n2[1].Kind);

        Assert.NotEqual(n1[0].Value.Ticks, n2[0].Value.Ticks);
        Assert.Equal(2, subscribeCalls);
        Assert.Equal(0, unsubscribeCalls);
    }

    [Fact]
    public async Task FatalAlertTest()
    {
        IObservable<IHasRequestId> observable = subject.ToObservableSingleWithRequestId(
            () => Id,
            requestId =>
            {
                Assert.Equal(Id, requestId);
                Interlocked.Increment(ref subscribeCalls);
                subject.OnError(new Alert(requestId, 1, "some error", true).ToException());
            },
            requestId =>
            {
                Assert.Equal(Id, requestId);
                Interlocked.Increment(ref unsubscribeCalls);
            });

        AlertException alertException = await Assert.ThrowsAsync<AlertException>(async () => await observable);

        Assert.IsType<Alert>(alertException.Alert);
        Assert.Equal(Id, alertException.Alert.RequestId);

        Assert.Equal(1, subscribeCalls);
        Assert.Equal(0, unsubscribeCalls);
    }

    [Fact]
    public async Task NonFatalAlertTest()
    {
        IObservable<IHasRequestId> observable = subject.ToObservableSingleWithRequestId(
            () => Id,
            requestId =>
            {
                Assert.Equal(Id, requestId);
                Interlocked.Increment(ref subscribeCalls);
                subject.OnNext(new Alert(requestId, 1, "some warning", false));
                subject.OnNext(new SomeClass());
            },
            requestId =>
            {
                Assert.Equal(Id, requestId);
                Interlocked.Increment(ref unsubscribeCalls);
            });

        IList<Notification<IHasRequestId>> n1 = await observable.Materialize().ToList();
        Assert.Equal(3, n1.Count);
        Assert.Equal(NotificationKind.OnNext, n1[0].Kind);
        Assert.Equal(NotificationKind.OnNext, n1[1].Kind);
        Assert.Equal(NotificationKind.OnCompleted, n1[2].Kind);

        Assert.Equal(1, subscribeCalls);
        Assert.Equal(0, unsubscribeCalls);
    }

    [Fact]
    public async Task SubscribeErrorTest()
    {
        IObservable<IHasRequestId> observable = subject.ToObservableSingleWithRequestId(
            () => Id,
            requestId => { throw new BarrierPostPhaseException(); },
            requestId => Interlocked.Increment(ref unsubscribeCalls));
        await Assert.ThrowsAsync<BarrierPostPhaseException>(async () => await observable);
        Assert.Equal(0, unsubscribeCalls);
    }

    [Fact]
    public async Task SourceErrorTest()
    {
        IObservable<IHasRequestId> observable = subject.ToObservableSingleWithRequestId(
            () => Id,
            requestId => subject.OnError(new BarrierPostPhaseException()),
            requestId => Interlocked.Increment(ref unsubscribeCalls));
        await Assert.ThrowsAsync<BarrierPostPhaseException>(async () => await observable);
        Assert.Equal(0, unsubscribeCalls);
    }

    [Fact]
    public async Task SourceCompletedTest()
    {
        IObservable<IHasRequestId> observable = subject.ToObservableSingleWithRequestId(
            () => Id,
            requestId => subject.OnCompleted(),
            requestId => Interlocked.Increment(ref unsubscribeCalls));
        await Assert.ThrowsAsync<InvalidOperationException>(async () => await observable);
        Assert.Equal(0, unsubscribeCalls);
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

        IObservable<IHasRequestId> observable = subject.ToObservableSingleWithRequestId(
            () => Id,
            requestId => Interlocked.Increment(ref subscribeCalls),
            requestId => Interlocked.Increment(ref unsubscribeCalls));

        await Assert.ThrowsAsync<TimeoutException>(async () => await observable.Timeout(ts));
        await Task.Delay(1);
        Assert.Equal(1, subscribeCalls);
        Assert.Equal(1, unsubscribeCalls);
    }

    [Fact]
    public void UnsubscribeErrorTest()
    {
        IObservable<IHasRequestId> observable = subject.ToObservableSingleWithRequestId(
            () => Id,
            requestId => Interlocked.Increment(ref subscribeCalls),
            requestId => { throw new BarrierPostPhaseException(); });

        Assert.Throws<BarrierPostPhaseException>(() => observable.Subscribe().Dispose());
        Assert.Equal(1, subscribeCalls);
    }
}
