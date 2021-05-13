using System;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using InterReact.Extensions;
using System.Collections.Generic;
using System.Text;

namespace InterReact.UnitTests.Experimental
{
    public class Test_Async : BaseTest
    {
        public Test_Async(ITestOutputHelper output) : base(output) { }

        public async Task<int> AsyncFunction(CancellationToken ct)
        {
            await Task.Delay(10000, ct); // some work
            return 1;
        }

        [Fact]
        public async Task Test()
        {
            var cts = new CancellationTokenSource();
            cts.CancelAfter(1);
            var token = cts.Token;
            try
            {
                var result = await AsyncFunction(token).ConfigureAwait(false);
                Write("ok");
            }
            catch (OperationCanceledException)
            {
                Write("cancelled");
            }
        }

    }

    public class Test_Observable : BaseTest
    {
        public Test_Observable(ITestOutputHelper output) : base(output) { }

        public IObservable<int> GetObservable()
        {
            return Observable.Create<int>(async (observer, ct) =>
            {
                try
                {
                    await Task.Delay(1000, ct);
                    observer.OnNext(1);
                    observer.OnCompleted();
                }
                catch (OperationCanceledException)
                {
                    // observer.OnError(e);
                    // is not necessary because upstream has already unsubscribed
                    ;
                }
            });
        }

        [Fact]
        public async Task Test()
        {
            var observable = GetObservable();
            var result = await observable;
            Assert.Equal(1, result);
        }

        [Fact]
        public async Task Test_Timeout()
        {
            var observable = GetObservable();
            try
            {
                var result = await observable.Timeout(TimeSpan.FromMilliseconds(10));
            }
            catch (TimeoutException e)
            {
                Write(e.Message);
            }
        }

        [Fact]
        public async Task Test_Cancellation()
        {
            var observable = GetObservable();
            var cts = new CancellationTokenSource();
            //cts.Cancel();
            cts.CancelAfter(100);
            try
            {
                var result = await observable.CancelWhen(cts.Token);
                Write("not cancelled");
            }
            catch (OperationCanceledException e)
            {
                Write(e.Message);
            }
        }

        /////////////////////////////////////////////////////////

        public byte b = 0;
        public async Task<byte> ReadByte()
        {
            b++;
            if (b < 5)
                return b;
            await Task.Delay(1);
            return 0;
        }

        public async Task<string> XReadString(Task<byte> read)
        {
            var list = new List<byte>();
            while (true)
            {
                var b = await read;
                if (b == 0)
                    return Encoding.UTF8.GetString(list.ToArray());
                list.Add(b);
            }
        }

        [Fact]
        public async Task TestTest()
        {
            //Task<string> xx = () => ReadByte();
            //var ss = await XReadString(() => ReadByte());
            var cts = new CancellationTokenSource();
            cts.Cancel();
            //cts.Cancel();
            //var result = await SomeTask(cts.Token);

            //var oo = await SomeTask(cts.Token).ToObservable().Spy(Write);

            var o = Observable.Return(42);
            //var oo = o.tas
            await Task.Delay(1);
            ;
        }
    }
}
