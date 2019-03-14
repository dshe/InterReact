using NodaTime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;

namespace InterReact.Utility.Rx
{
    internal static class AsyncSourceEx
    {
        /// <summary>
        /// Returns an observable that shares a single subscription to an underlying sequence which produces a single result.
        /// The source is connected upon first subscription.
        /// Used by ContractDetails.
        /// </summary>
        internal static IObservable<T> ToAsyncSource<T>(this IObservable<T> source, Duration lifetime, IClock clock)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            var gate = new object();
            AsyncSubject<T>? subject = null;
            IDisposable? connection = null;

            var updated = Instant.MinValue;
            var complete = false;

            return Observable.Create<T>(observer =>
            {
                lock (gate)
                {
                    if (subject == null || ((clock.GetCurrentInstant() - updated > lifetime || !complete) && !subject.HasObservers))
                    {
                        subject?.Dispose();
                        subject = new AsyncSubject<T>();
                        complete = false;
                        updated = clock.GetCurrentInstant();
                        connection?.Dispose();
                        connection = source
                            .Subscribe(
                                onNext: subject.OnNext,
                                onError: subject.OnError,
                                onCompleted: () =>
                                {
                                    lock (gate)
                                    {
                                        complete = true;
                                        subject.OnCompleted();
                                    }
                                });
                    }

                    var subscription = subject.Subscribe(observer);

                    return Disposable.Create(() =>
                    {
                        lock (gate)
                            subscription.Dispose();
                    });
                }
            });
        }
    }

}
