using System;
using System.Diagnostics;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using InterReact.Extensions;
using Xunit;
using Xunit.Abstractions;

namespace InterReact.UnitTests.Extensions
{
    public class ToObservableTest : BaseTest
    {
        public ToObservableTest(ITestOutputHelper output) : base(output) { }

        private int subscribeCalls, unsubscribeCalls;
        private readonly Subject<object> subject = new Subject<object>();

        public class SomeClass
        {
            public long Time { get; } = Stopwatch.GetTimestamp();
        }

        public class SomeClassEnd { }


        [Fact]
        public async Task Test_Ok()
        {
            var observable = subject.ToObservable<SomeClass>(
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
            Assert.Equal(0, unsubscribeCalls);
        }

        [Fact]
        public async Task Test_Multi_Ok()
        {
            var observable = subject.ToObservable<SomeClass, SomeClassEnd>(
                () =>
                {
                    Interlocked.Increment(ref subscribeCalls);
                    subject.OnNext(new SomeClass());
                    subject.OnNext(new SomeClass());
                    subject.OnNext(new SomeClassEnd());
                },
                () =>
                    Interlocked.Increment(ref unsubscribeCalls));
            var list = await observable.ToList();
            Assert.Equal(2, list.Count);
            Assert.Equal(1, subscribeCalls);
            Assert.Equal(0, unsubscribeCalls);
        }

        [Fact]
        public async Task Test_Subscribe_Error()
        {
            var observable = subject.ToObservable<string>(() => { throw new BarrierPostPhaseException(); });
            await Assert.ThrowsAsync<BarrierPostPhaseException>(async () => await observable);
        }

        [Fact]
        public async Task Test_Source_Error()
        {
            var observable = subject.ToObservable<string>(() => subject.OnError(new BarrierPostPhaseException()));
            await Assert.ThrowsAsync<BarrierPostPhaseException>(async () => await observable);
        }

        [Fact]
        public async Task Test_Source_Completed()
        {
            var observable = subject.ToObservable<string>(() => subject.OnCompleted());
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () => await observable);
            Assert.Equal("Sequence contains no elements.", ex.Message);
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
            var observable = subject.ToObservable<string>(() => Interlocked.Increment(ref subscribeCalls), () => Interlocked.Increment(ref unsubscribeCalls));
            await Assert.ThrowsAsync<TimeoutException>(async () => await observable.Timeout(ts));
            await Task.Delay(1);
            Assert.Equal(1, subscribeCalls);
            Assert.Equal(1, unsubscribeCalls);
        }
    }

}
