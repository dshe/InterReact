using System;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using InterReact.Utility.Rx;
using Stringification;
using InterReact.Tests.Utility;
using Microsoft.Reactive.Testing;
using Xunit;
using Xunit.Abstractions;
using NodaTime;
using Microsoft.Extensions.Logging;

namespace InterReact.Tests.UnitTests.Rx
{
    public class ToReplaySourceTests : BaseReactiveTest
    {
        private readonly TestScheduler testScheduler = new TestScheduler();

        public  ToReplaySourceTests(ITestOutputHelper output) : base(output) {}

        [Fact]
        public void T01_Consecutive()
        {
            var observer1 = testScheduler.CreateObserver<string>();
            var observer2 = testScheduler.CreateObserver<string>();
            var observable = testScheduler.CreateColdObservable(
                OnNext(100, "1"),
                OnCompleted<string>(1000)
            );

            var sharedObservable = observable.ToAsyncSource(Duration.MaxValue, SystemClock.Instance);

            sharedObservable.SubscribeOn(testScheduler).Subscribe(observer1);

            testScheduler.AdvanceBy(1000);

            sharedObservable.SubscribeOn(testScheduler).Subscribe(observer2);

            testScheduler.Start();

            var expected = new[]
            {
                //OnNext(101, "1"),
                OnNext(1001, "1"),
                OnCompleted<string>(1001)
            };
            Assert.Equal(expected.ToList(), observer1.Messages);

            expected = new[]
            {
                OnNext(1001, "1"),
                OnCompleted<string>(1001)
            };
            Assert.Equal(expected.ToList(), observer2.Messages);

            Assert.Equal(1, observable.Subscriptions.Count);
            Logger.LogDebug(observable.Subscriptions.Stringify());
        }

        [Fact]
        public void T02_Overlapping()
        {
            var observer1 = testScheduler.CreateObserver<string>();
            var observer2 = testScheduler.CreateObserver<string>();
            var observable = testScheduler.CreateColdObservable(
                OnNext(100, "1"),
                OnNext(200, "2"),
                OnCompleted<string>(1000)
            );
            var sharedObservable = observable.ToAsyncSource(Duration.MaxValue, SystemClock.Instance);

            sharedObservable.SubscribeOn(testScheduler).Subscribe(observer1);

            testScheduler.AdvanceBy(150);

            sharedObservable.SubscribeOn(testScheduler).Subscribe(observer2);

            testScheduler.Start();

            var expected = new[]
            {
                //OnNext(101, "1"),
                //OnNext(201, "2"),
                OnNext(1001, "2"),
                OnCompleted<string>(1001)
            };
            Assert.Equal(expected.ToList(), observer1.Messages);

            expected = new[]
            {
                //OnNext(151, "1"),
                //OnNext(201, "2"),
                OnNext(1001, "2"),
                OnCompleted<string>(1001)
            };
            Assert.Equal(expected.ToList(), observer2.Messages);

            Assert.Equal(1, observable.Subscriptions.Count);
            Logger.LogDebug(observable.Subscriptions.Stringify());
        }

        [Fact]
        public void T03_Concurrent()
        {
            var observer1 = testScheduler.CreateObserver<string>();
            var observer2 = testScheduler.CreateObserver<string>();
            var observable = testScheduler.CreateColdObservable(
                OnNext(100, "1"),
                OnCompleted<string>(1000)
            );
            var sharedObservable = observable.ToAsyncSource(Duration.MaxValue, SystemClock.Instance);

            sharedObservable.SubscribeOn(testScheduler).Subscribe(observer1);
            sharedObservable.SubscribeOn(testScheduler).Subscribe(observer2);

            testScheduler.Start();

            var expected = new[]
            {
               // OnNext(101, "1"),
                OnNext(1001, "1"),

                OnCompleted<string>(1001)
            };
            Assert.Equal(expected.ToList(), observer1.Messages);
            Assert.Equal(expected.ToList(), observer2.Messages);
            Assert.Equal(1, observable.Subscriptions.Count);
            Logger.LogDebug(observable.Subscriptions.Stringify());
        }


        [Fact]
        public async Task T04_Lifetime_zero()
        {
            var calls = 0;
            var observable = Observable.Create<int>(observer =>
            {
                observer.OnNext(calls++);
                observer.OnCompleted();
                return Disposable.Empty;
            }).ToAsyncSource(Duration.Zero, SystemClock.Instance);
            await observable;
            await observable;
            Assert.Equal(2, calls);
        }

        [Fact]
        public async Task T05_Source_Retry_Error()
        {
            var observable = Observable.Create<string>(observer =>
            {
                observer.OnError(new BarrierPostPhaseException(DateTime.UtcNow.Ticks.ToString()));
                return Disposable.Empty;
            }).ToAsyncSource(Duration.MaxValue, SystemClock.Instance);

            var e1 = await Assert.ThrowsAsync<BarrierPostPhaseException>(async () => await observable);
            await Task.Delay(1);
            var e2 = await Assert.ThrowsAsync<BarrierPostPhaseException>(async () => await observable);
            Assert.NotEqual(e1.Message, e2.Message);
        }

    }

}
