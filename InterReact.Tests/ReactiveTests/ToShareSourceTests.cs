using InterReact.Extensions;
using Microsoft.Reactive.Testing;
using Stringification;
using System.Linq;
using System.Reactive.Linq;
using Xunit;
using Xunit.Abstractions;

namespace InterReact.UnitTests.Extensions
{
    public sealed class ToShareSourceTests : BaseReactiveTest
    {
        public ToShareSourceTests(ITestOutputHelper output) : base(output) { }

        [Fact]
        public void T01_Consecutive()
        {
            var testScheduler = new TestScheduler();
            var observer1 = testScheduler.CreateObserver<string>();
            var observer2 = testScheduler.CreateObserver<string>();
            var observable = testScheduler.CreateColdObservable(
                OnNext(100, "1"),
                OnCompleted<string>(1000)
            );
            var sharedObservable = observable.ToShareSource();

            sharedObservable.SubscribeOn(testScheduler).Subscribe(observer1);

            testScheduler.AdvanceBy(1000);

            sharedObservable.SubscribeOn(testScheduler).Subscribe(observer2);

            testScheduler.Start();

            var expected = new[]
            {
                OnNext(101, "1"),
                OnCompleted<string>(1001)
            };
            Assert.Equal(expected.ToList(), observer1.Messages);

            expected = new[]
            {
                OnNext(1101, "1"),
                OnCompleted<string>(2001)
            };
            Assert.Equal(expected.ToList(), observer2.Messages);

            Assert.Equal(2, observable.Subscriptions.Count);
            Write(observable.Subscriptions.Stringify());
        }

        [Fact]
        public void T02_Overlapping()
        {
            var testScheduler = new TestScheduler();
            var observer1 = testScheduler.CreateObserver<string>();
            var observer2 = testScheduler.CreateObserver<string>();
            var observable = testScheduler.CreateColdObservable(
                OnNext(100, "1"),
                OnNext(200, "2"),
                OnNext(300, "3"),
                OnNext(400, "4"),
                OnNext(500, "5"),
                OnCompleted<string>(600));
            var sharedObservable = observable.ToShareSource();

            sharedObservable.SubscribeOn(testScheduler).Subscribe(observer1);

            testScheduler.AdvanceBy(350);

            sharedObservable.SubscribeOn(testScheduler).Subscribe(observer2);

            testScheduler.Start();

            var expected = new[]
            {
                OnNext(101, "1"),
                OnNext(201, "2"),
                OnNext(301, "3"),
                OnNext(401, "4"),
                OnNext(501, "5"),
                OnCompleted<string>(601)
            };
            Assert.Equal(expected.ToList(), observer1.Messages);

            expected = new[]
            {
                OnNext(351, "1"),
                OnNext(351, "2"),
                OnNext(351, "3"),
                OnNext(401, "4"),
                OnNext(501, "5"),
                OnCompleted<string>(601)
            };
            Assert.Equal(expected.ToList(), observer2.Messages);
            Assert.Equal(1, observable.Subscriptions.Count);
        }

        [Fact]
        public void T03_Concurrent()
        {
            var testScheduler = new TestScheduler();
            var observer1 = testScheduler.CreateObserver<string>();
            var observer2 = testScheduler.CreateObserver<string>();
            var observable = testScheduler.CreateColdObservable(
                OnNext(100, "1"),
                OnNext(200, "2"),
                OnNext(300, "3"),
                OnNext(400, "4"),
                OnNext(500, "5"),
                OnCompleted<string>(600));
            var sharedObservable = observable.ToShareSource();

            sharedObservable.SubscribeOn(testScheduler).Subscribe(observer1);
            sharedObservable.SubscribeOn(testScheduler).Subscribe(observer2);

            testScheduler.Start();

            var expected = new[]
            {
                OnNext(101, "1"),
                OnNext(201, "2"),
                OnNext(301, "3"),
                OnNext(401, "4"),
                OnNext(501, "5"),
                OnCompleted<string>(601)
            };
            Assert.Equal(expected.ToList(), observer1.Messages);
            Assert.Equal(expected.ToList(), observer2.Messages);
            Assert.Equal(1, observable.Subscriptions.Count);
        }

    }
}
