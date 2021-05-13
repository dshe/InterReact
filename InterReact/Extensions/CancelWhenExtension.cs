using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading;

namespace InterReact.Extensions
{
    public static class CancelWhenExtension
    {
        public static IObservable<T> CancelWhen<T>(this IObservable<T> source, CancellationToken ct)
        {
            return Observable.Create<T>(observer =>
            {
                var subscription = source.Subscribe(observer);

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
