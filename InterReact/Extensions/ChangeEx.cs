using InterReact.Utility;
using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;

#nullable enable

namespace InterReact.Extensions
{
    public static class ChangeEx
    {
        /// <summary>
        /// Returns an observable sequence of arrays which contain the current and previous source message,
        /// except for the first array in the sequence, which contains only the source message.
        /// Buffer(2,1) is not used because it does not emit the first value (when there is no previous value).
        /// The first value is useful for, for example, initializing display objects.
        /// </summary>
        private static IObservable<T[]> PairWithPrevious<T>(this IObservable<T> source)
        {
            return Observable.Create<T[]>(observer =>
            {
                var first = true;
                var previous = default(T);

                var subscription = source.Subscribe(
                    onNext: m =>
                    {
                        if (first)
                        {
                            first = false;
                            observer.OnNext(new[] {m});
                        }
                        else
                            observer.OnNext(new[] {m, previous});
                        previous = m;

                    },
                    onError: observer.OnError,
                    onCompleted: observer.OnCompleted);

                return Disposable.Create(() =>
                {
                    subscription.Dispose();
                    first = true;
                });

            });
        }

        /// <summary>
        /// Returns an observable sequence of changes in value, except for the first value, which is null.
        /// </summary>
        public static IObservable<double?> Change(this IObservable<double> source)
        {
            return ThrowIf.ThrowIfNull(source)
                .DistinctUntilChanged()
                .PairWithPrevious()
                .Select(m => m.Length == 2 && m[0] > 0 && m[1] > 0 ? m[0] - m[1] : (double?)null);
        }

        public static IObservable<T> WhereHasValue<T>(this IObservable<T?> source) where T : struct
            => source.Where(x => x.HasValue).Select(x => x.GetValueOrDefault());

    }
}
