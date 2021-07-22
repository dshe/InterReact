using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace InterReact.UnitTests.Experimental
{
    public class TestClass : UnitTestsBase
    {
        public TestClass(ITestOutputHelper output) : base(output) { }

        [Fact]
        public async Task Test1()
        {
            var observable = Observable.Create<string>(async observer =>
            {
                Logger.LogDebug("subscribing");
                await Task.Delay(200);
                observer.OnNext("1");
                await Task.Delay(200);
                observer.OnNext("2");
                await Task.Delay(200);
                observer.OnNext("3");
                await Task.Delay(200);
                observer.OnCompleted();

                return Disposable.Empty;
            }).ObserveOn(Scheduler.Default).Replay().RefCount();

            observable.Subscribe(x => Logger.LogDebug(x));

            await Task.Delay(1500);

            observable.Subscribe(x => Logger.LogDebug(x));

            await Task.Delay(2000);


        }

    }
}
