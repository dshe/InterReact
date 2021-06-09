using System;
using System.Linq;
using System.Collections;
using System.IO;
using System.Reactive.Linq;
using System.Reactive;

namespace InterReact
{
    public static class ThrowIfAnyEmptyExtensions
    {
        public static IObservable<T> ThrowIfAnyEmpty<T>(this IObservable<T> source) where T : IEnumerable
        {
            return Observable.Create<T>(observer =>
            {
                return source.SubscribeSafe(Observer.Create<T>(
                    onNext: m =>
                    {
                        if (m.GetEnumerator().MoveNext())
                            observer.OnNext(m);
                        else
                            observer.OnError(new InvalidDataException("Empty collection."));
                    },
                    onError: observer.OnError,
                    onCompleted: observer.OnCompleted));
            });
        }
    }
}
