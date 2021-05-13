using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Reactive.Testing;
using Xunit;
using Xunit.Abstractions;
using Stringification;
using InterReact.Extensions;

namespace InterReact.UnitTests.Extensions
{
    public class ToCacheSourceTests : BaseReactiveTest
    {
        public ToCacheSourceTests(ITestOutputHelper output) : base(output) { }

        [Fact]
        public void T00_Test()
        {
            var scheduler = new TestScheduler();
            var observer1 = scheduler.CreateObserver<string>();
            var observer2 = scheduler.CreateObserver<string>();

            var source = scheduler.CreateHotObservable(
                OnNext(100, "one"),
                OnNext(200, "two"),
                OnNext(300, "three"));

            var published = source.ToCacheSource(x => x);
            published.Connect();
            var observable = published;

            observable.Subscribe(observer1);
            scheduler.AdvanceBy(150);
            observable.Subscribe(observer2);

            var expected1 = new[] { OnNext(100, "one") };
            Assert.Equal(expected1.ToList(), observer1.Messages);
            var expected2 = new[] { OnNext(150, "one") };
            Assert.Equal(expected2.ToList(), observer2.Messages);

            scheduler.Start();
            Assert.Equal(3, observer1.Messages.Count);
            Assert.Equal(3, observer2.Messages.Count);
        }

        [Fact]
        public async Task T01_Empty()
        {
            var observable = Observable.Empty<string>().ToCacheSource(x => x); // completes
            observable.Connect();
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () => await observable);
            Assert.Equal("Sequence contains no elements.", ex.Message);
        }

        [Fact]
        public async Task T02_Dispose_Connection()
        {
            var observable = AutoObservable.Create<string>(int.MaxValue, TimeSpan.FromMilliseconds(10))
                .ToCacheSource(x => x);
            var mre = new ManualResetEventSlim();

            observable.Subscribe(x => mre.Set());

            observable.Connect();
            mre.Wait();

            observable.Connect().Dispose();
            mre.Reset();
            await Task.Delay(100);
            Assert.False(mre.IsSet);
        }

        [Fact]
        public async Task T03_Cache()
        {
            var source = new Subject<string>();
            var observable = source.ToCacheSource(x => x);

            observable.Connect();

            source.OnNext("1");
            source.OnNext("2");
            source.OnNext("3");
            source.OnNext("2"); // duplicate key

            var list = await observable.Take(TimeSpan.FromMilliseconds(10)).ToList();
            Assert.Equal(3, list.Count);

            source.OnNext("10");
            list = await observable.Take(TimeSpan.FromMilliseconds(10)).ToList();
            Assert.Equal(4, list.Count);
        }

        [Fact]
        public void T04_Throw_In_OnNext()
        {
            var source = new Subject<string>();
            var observable = source.ToCacheSource(x => x);

            observable.Subscribe(x =>
                throw new BarrierPostPhaseException("some exception"));

            observable.Connect();

            Assert.Throws<BarrierPostPhaseException>(() => source.OnNext("message"));
        }

        [Fact]
        public async Task T05_Concurrency()
        {
            const int numberOfTasks = 17;
            const int numberOfValues = 7;
            var intervalBetweenValues = TimeSpan.FromMilliseconds(2);
            var intervalBetweenTasks = TimeSpan.FromMilliseconds(3);

            var source = AutoObservable.Create<string>(count: numberOfValues, delay: intervalBetweenValues)
                .ToCacheSource(x => x);

            source.Connect();

            var tasks = new List<Task<IList<string>>>();
            for (var i = 0; i < numberOfTasks; i++)
            {
                await Task.Delay(intervalBetweenTasks);
                tasks.Add(source.ToList().ToTask());
            }
            var ok = true;
            foreach (var task in tasks)
            {
                var result = await task.ConfigureAwait(false);
                if (result.Count != numberOfValues)
                    ok = false;
                Write($"{(result.Count == numberOfValues ? "Ok" : "FAILURE")} {result.Count}");
            }
            Assert.True(ok);
        }

    }

}
