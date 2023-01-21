using System.Diagnostics;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Utility;

public class ToObservableContinuousWithId : UnitTestBase
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

    public ToObservableContinuousWithId(ITestOutputHelper output) : base(output) { }

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
}
