using System;
using System.Reactive.Linq;

namespace InterReact.Extensions
{
    public static class TimeoutExtension
    {
        public static IObservable<T> Timeout<T>(
            this IObservable<T> source, TimeSpan timespan, string? message = null) =>

                source.Timeout(timespan, Observable.Throw<T>(new TimeoutException(message)));
    }
}
