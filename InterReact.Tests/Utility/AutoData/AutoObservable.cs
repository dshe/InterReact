using System;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace InterReact.Tests.Utility.AutoData
{
    public static class AutoObservable
    {
        /// <summary>
        /// Returns an observable that emits the specified number of elements.
        /// </summary>
        /// <typeparam name="T">The type Values emitted by the observable.</typeparam>
        /// <param name="count">
        /// The number of elements emitted by the observable before completion.
        /// If count is zero, the sequence does not emit any values and completes immediately.
        /// If count is Int32.MaxValue, the sequence emits values continuously until an observer unsubscribes.
        /// </param>
        /// <param name="delay">
        /// The duration between emitted Values.
        /// If delay is equal to default(TimeSpan), which is also equal to TimeSpan.Zero, values are emitted as quickly as possible.
        /// if delay is equal to TimeSpan.Maxvalue, no values are emitted and the sequence never completes.
        /// </param>
        public static IObservable<T> Create<T>(int count, TimeSpan delay = default) where T : class
        {
            return Observable.Create<T>(observer => NewThreadScheduler.Default.ScheduleLongRunning(async ct =>
                {
                    for (var i = 0; i < count || count == int.MaxValue; i++)
                    {
                        if (ct.IsDisposed) // the subscription has already been disposed so we can't send any more messages
                            return;
                        try
                        {
                            observer.OnNext(AutoData.Create<T>());
                        }
                        catch (Exception ex)
                        {
                            observer.OnError(ex);
                            return;
                        }
                        await Task.Delay(delay);
                    }
                    observer.OnCompleted();
                })
            );
        }
    }
}
