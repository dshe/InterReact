using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;
using Stringification;
using Xunit;
using Xunit.Abstractions;

namespace InterReact.Tests.Utility.AutoData
{
    public class AutoObservableTests : BaseUnitTest
    {
        public AutoObservableTests(ITestOutputHelper output) : base(output) { }

        [Fact]
        public async Task Test_Error()
        {
            var observable = Observable.Throw<string>(new ArithmeticException());
            await Assert.ThrowsAsync<ArithmeticException>(async () => await observable);
            await Assert.ThrowsAsync<ArithmeticException>(async () => await observable);
        }

        [Fact]
        public async Task Test_Timeout()
        {
            var observable1 = Observable.Never<string>();
            await Assert.ThrowsAsync<TimeoutException>(async () => await observable1.Timeout(TimeSpan.FromMilliseconds(1)));

            var observable2 = AutoObservable.Create<string>(count: int.MaxValue, delay: TimeSpan.FromMilliseconds(100));
            await Assert.ThrowsAsync<TimeoutException>(async () => await observable2.Timeout(TimeSpan.FromMilliseconds(1)));
        }

        [Fact]
        public async Task Test_Empty()
        {
            var observable1 = Observable.Empty<string>();
            var ex1 = await Assert.ThrowsAsync<InvalidOperationException>(async () => await observable1);

            var observable2 = AutoObservable.Create<string>(count:0);
            var ex2 = await Assert.ThrowsAsync<InvalidOperationException>(async () => await observable2);
            Logger.LogDebug(ex1.Message);
            Assert.Equal(ex1.Message, ex2.Message);
        }

        [Fact]
        public async Task Test_Single()
        {
            var observable1 = Observable.Return("x");
            await observable1.SingleAsync();

            var observable = AutoObservable.Create<string>(count:1);
            await observable.SingleAsync();
        }

        [Fact]
        public async Task Test_Many()
        {
            const int count = 11;

            var observables1 = Observable.Range(1, count);
            var list1 = await observables1.ToList();
            Assert.Equal(count, list1.Count);
           
            var observable = AutoObservable.Create<string>(count);
            var list = await observable.ToList();
            Assert.Equal(count, list.Count);
        }

        [Fact]
        public async Task Test_Concurrent()
        {
            const int count = 11;
            const int taskNo = 7;

            var observable = AutoObservable.Create<string>(count, TimeSpan.FromMilliseconds(1));
            var tasks = new List<Task<IList<string>>>();
            for (var i = 0; i < taskNo; i++)
            {
                tasks.Add(observable.ToList().ToTask());
                await Task.Delay(2);
            }
            var allOk = true;
            foreach (var task in tasks)
            {
                var list = await task;
                if (list.Count != count)
                {
                    allOk = false;
                    Logger.LogDebug("FAILURE " + list.Count);
                }
            }
            Assert.True(allOk);
        }

        [Fact]
        public async Task Test_Publish()
        {
            var connectableObservable = AutoObservable.Create<string>(int.MaxValue, TimeSpan.FromMilliseconds(10))
                .Publish();
            await Assert.ThrowsAsync<TimeoutException>(async () => await connectableObservable.FirstAsync().Timeout(TimeSpan.FromMilliseconds(50)));
            var mre = new ManualResetEventSlim();
            connectableObservable.Subscribe(x => mre.Set());
            await Assert.ThrowsAsync<TimeoutException>(async () => await connectableObservable.FirstAsync().Timeout(TimeSpan.FromMilliseconds(50)));
            Assert.False(mre.IsSet);

            var connection = connectableObservable.Connect();
            await connectableObservable.FirstAsync();
            Assert.True(mre.IsSet);

            connection.Dispose();
            mre.Reset();
            connectableObservable.Connect();
            await connectableObservable.FirstAsync();
            Assert.True(mre.IsSet); // Connected().Dispose() does not dispose subscriptions!
            await Task.Delay(100);
        }

        [Fact]
        public async Task Test_Unsubscribe_Publish()
        {
            var connectableObservable = AutoObservable.Create<string>(int.MaxValue, TimeSpan.FromMilliseconds(10)).Publish();
            var connection = connectableObservable.Connect();

            var subscription = connectableObservable.Subscribe(
                onNext: message => Logger.LogDebug("OnNext: " + message.Stringify()),
                onError: ex => Logger.LogDebug("OnError: " + ex.Message),
                onCompleted: () => Logger.LogDebug("OnCompleted!"));
            
            await Task.Delay(100);
            //subscription.Dispose();

            connection.Dispose(); // OnCompleted is not called

            await Task.Delay(100);
        }

    }
}
