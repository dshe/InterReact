using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace InterReact.UnitTests.Experimental
{
    public class Test_Async : UnitTestsBase
    {
        public Test_Async(ITestOutputHelper output) : base(output) { }

        [Fact]
        public async Task Test()
        {
            IObservable<string> observable = Observable.Create<string>(async observer =>
            {
                for (int i = 0; i < 10; i++)
                {
                    observer.OnNext(i.ToString());
                    Write(i.ToString());
                    await Task.Delay(100);
                }
                observer.OnCompleted();
                return Disposable.Empty;
            });

            // ToTask() is required in order to start the observable
            var obs = observable.FirstAsync().ToTask();

            await Task.Delay(2000);

            var xx = await obs;
            ;

        }
    }

}
