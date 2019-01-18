using System;
using System.Net;
using System.Net.Sockets;
using System.Reactive;
using System.ComponentModel;
using System.Reactive.Disposables;
using System.Threading;
using System.Reactive.PlatformServices;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Concurrency;


namespace InterReact.Utility.Rx
{
    internal static class ShareSourceEx
    {
        /// <summary>
        /// Returns an observable that relays messages from an underlying sequence so that concurrent observers, if any, receive the same sequence.
        /// Used by: ManagedAccounts, AccountPositions, AccountSummary, TickSnapshot, ScannerParameters, FundamentalData.
        /// </summary>
        internal static IObservable<T> ToShareSource<T>(this IObservable<T> source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            var gate = new object();
            ReplaySubject<T> subject = null;
            IDisposable connection = null;

            return Observable.Create<T>(observer =>
            {
                lock (gate)
                {
                    if (subject == null || subject.IsDisposed)
                        subject = new ReplaySubject<T>();
                    var subscription = subject.Subscribe(observer);
                    if (connection == null)
                        connection = source.Subscribe(subject);

                    return Disposable.Create(() =>
                    {
                        lock (gate)
                        {
                            subscription.Dispose();
                            if (subject.HasObservers)
                                return;
                            connection?.Dispose();
                            connection = null;
                            subject.Dispose();
                        }
                    });
                }
            });
        }
    }

}
