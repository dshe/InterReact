using System.Diagnostics;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Utility;

public class ToObservableMultipleWithId : UnitTestBase
{
    private const int Id = 42;
    private int subscribeCalls;
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

    public ToObservableMultipleWithId(ITestOutputHelper output) : base(output) { }


    [Fact]
    public async Task MultiOkTest()
    {
        IObservable<IHasRequestId> observable = subject.ToObservableMultipleWithRequestId<SomeClassEnd>(
            () => Id,
            requestId =>
            {
                Assert.Equal(Id, requestId);
                Interlocked.Increment(ref subscribeCalls);
                subject.OnNext(new SomeClass());
                subject.OnNext(new SomeClass());
                subject.OnNext(new SomeClassEnd());
            });
        IList<IHasRequestId> list = await observable.ToList();
        Assert.Equal(2, list.Count);
        Assert.Equal(1, subscribeCalls);
    }

    [Fact]
    public async Task NonFatalAlertMultiTest()
    {
        IObservable<IHasRequestId> observable = subject.ToObservableMultipleWithRequestId<SomeClassEnd>(
            () => Id,
            requestId =>
            {
                Assert.Equal(Id, requestId);
                Interlocked.Increment(ref subscribeCalls);
                subject.OnNext(new Alert(requestId, 1, "some error", false));
                subject.OnNext(new SomeClass());
                subject.OnNext(new SomeClass());
                subject.OnNext(new SomeClassEnd());
            });
        IList<IHasRequestId> list = await observable.ToList();
        Assert.Equal(3, list.Count);
        Assert.Equal(1, subscribeCalls);
    }

    [Fact]
    public async Task FatalAlertMultiTest()
    {
        IObservable<IHasRequestId> observable = subject.ToObservableMultipleWithRequestId<SomeClassEnd>(
            () => Id,
            requestId =>
            {
                Assert.Equal(Id, requestId);
                Interlocked.Increment(ref subscribeCalls);
                subject.OnNext(new SomeClass());
                subject.OnError(new Alert(requestId, 1, "some error", true).ToException());
            });

        AlertException alertException = await Assert.ThrowsAsync<AlertException>(async () => await observable);
        Assert.Equal(1, subscribeCalls);
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
