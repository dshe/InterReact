using System.Diagnostics;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace InterReact.UnitTests.Utility;

public class ToObservableContinuousWithIdTest : UnitTestsBase
{
    private const int Id = 42;
    private int subscribeCalls, unsubscribeCalls;
    private readonly Subject<object> subject = new Subject<object>();

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

    public ToObservableContinuousWithIdTest(ITestOutputHelper output) : base(output) { }

    [Fact]
    public async Task Test_Ok()
    {
        var observable = subject.ToObservableSingleWithId(
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

        var n1 = await observable.Materialize().ToList();
        Assert.Equal(2, n1.Count);
        Assert.Equal(NotificationKind.OnNext, n1[0].Kind);
        Assert.Equal(NotificationKind.OnCompleted, n1[1].Kind);

        var n2 = await observable.Materialize().ToList();
        Assert.Equal(2, n2.Count);
        Assert.Equal(NotificationKind.OnNext, n2[0].Kind);
        Assert.Equal(NotificationKind.OnCompleted, n2[1].Kind);

        Assert.NotEqual(n1[0].Value.Ticks, n2[0].Value.Ticks);
        Assert.Equal(2, subscribeCalls);
        Assert.Equal(0, unsubscribeCalls);
    }
}
