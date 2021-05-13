using System;
using System.Collections;
using System.IO;
using System.Reactive.Linq;

namespace InterReact.Extensions
{
    public static class ThrowIfExtensions
    {
        public static T ThrowIfNull<T>(this T source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            return source;
        }

        public static IObservable<T> ThrowIfAnyEmpty<T>(this IObservable<T> source) where T : IEnumerable
        {
            return Observable.Create<T>(observer =>
            {
                return source.Subscribe(
                    onNext: m =>
                    {
                        if (m.GetEnumerator().MoveNext())
                            observer.OnNext(m);
                        else
                            observer.OnError(new InvalidDataException("Empty collection."));
                    },
                    onError: observer.OnError,
                    onCompleted: observer.OnCompleted);
            });
        }
    }
}
