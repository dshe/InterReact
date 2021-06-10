using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading;

namespace InterReact
{
    public static partial class Extensions
    {
        public static IObservable<T> CancelWhen<T>(this IObservable<T> source, CancellationToken ct)
        {
            return Observable.Create<T>(observer =>
            {
                IDisposable subscription = source.SubscribeSafe(observer);

                CancellationTokenRegistration? regx = null;
                regx = ct.Register(() =>
                {
                    subscription.Dispose();
                    observer.OnError(new OperationCanceledException(ct));
                    regx?.Dispose();
                });

                return Disposable.Create(() =>
                {
                    subscription.Dispose();
                    regx?.Dispose();
                });
            });
        }
    }
}
