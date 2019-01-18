using System;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;

namespace InterReact.Extensions
{
    public static class TimeoutsEx
    {
        public static IObservable<T> Timeout<T>(this IObservable<T> source, TimeSpan timeout, string message = null) =>
            source.Timeout(timeout, Observable.Throw<T>(new TimeoutException(message)));

        public static Task<T> Timeout<T>(this Task<T> task, TimeSpan timeout, string message = null) =>
            task
            .ToObservable()
            .Timeout(timeout, Observable.Throw<T>(new TimeoutException(message)))
            .ToTask();

        public static Task<T> Timeout<T>(this Task<T> task, int seconds = 10, string message = null) =>
            Timeout(task, TimeSpan.FromSeconds(seconds), message);
    }

}
