using System;
using System.Reactive;
using System.Reactive.Linq;

namespace InterReact
{
    public static partial class Extensions
    {
        /// <summary>
        /// Returns an observable sequence of arrays which contain the current and previous source message,
        /// except for the first array in the sequence, which contains only the source message.
        /// Buffer(2,1) is not used because it does not emit the first value (when there is no previous value).
        /// The first value is useful for, for example, initializing display objects.
        /// </summary>
        private static IObservable<T[]> PairWithPrevious<T>(this IObservable<T> source) //where T : notnull
        {
            bool first = true;
            T? previous = default;

            return Observable.Create<T[]>(observer =>
            {
                return source.SubscribeSafe(Observer.Create<T>(
                    onNext: m =>
                    {
                        if (first)
                        {
                            first = false;
                            observer.OnNext(new[] { m });
                        }
                        else
                            observer.OnNext(new[] { m, previous! });
                        previous = m;
                    },
                    onError: observer.OnError,
                    onCompleted: observer.OnCompleted));
            });
        }

        /// <summary>
        /// Returns an observable sequence of changes in value, except for the first value, which is NaN.
        /// </summary>
        public static IObservable<double> Change(this IObservable<double> source)
        {
            return source
                .DistinctUntilChanged()
                .PairWithPrevious()
                .Select(m => m.Length == 2 && m[0] > 0 && m[1] > 0 ? m[0] - m[1] : double.NaN);
        }
    }
}
