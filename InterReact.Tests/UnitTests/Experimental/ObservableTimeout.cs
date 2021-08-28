using Microsoft.Extensions.Logging;
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
    public class Observable_Timeout : UnitTestsBase
    {
        public Observable_Timeout(ITestOutputHelper output) : base(output) { }

        [Fact]
        public async Task ObservableTimeout()
        {
            IObservable<string> observable = Observable.Create<string>(async observer =>
            {
                for (int i = 0; i < 10; i++)
                {
                    observer.OnNext(i.ToString());
                    Logger.LogInformation(i.ToString());
                    await Task.Delay(200);
                }
                observer.OnCompleted();
                return Disposable.Empty;
            });

            try
            {
                var obs = await observable.ToList().Timeout(TimeSpan.FromMilliseconds(500));
                Logger.LogInformation("complete");
            }
            catch (Exception e)
            {
                // Timeout cancels the observable
                Logger.LogInformation(e.Message);
            }

        }

        [Fact]
        public async Task ObservableCancelwithTask()
        {
            IObservable<string> observable = Observable.Create<string>(async observer =>
            {
                for (int i = 0; i < 10; i++)
                {
                    observer.OnNext(i.ToString());
                    Logger.LogInformation(i.ToString());
                    await Task.Delay(200);
                }
                observer.OnCompleted();
                return Disposable.Empty;
            });

            try
            {

                var cts = new CancellationTokenSource(500);
                var obs = await observable.ToList().ToTask(cts.Token);
                Logger.LogInformation("complete");
            }
            catch (Exception e)
            {
                // Cts cancels the observable
                Logger.LogInformation(e.Message);
            }
        }
    }
}
