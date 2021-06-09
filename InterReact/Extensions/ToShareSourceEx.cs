using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace InterReact
{
    public static partial class Extensions
    {
        /// <summary>
        /// Returns an observable that relays messages from an underlying sequence so that concurrent observers, if any, receive the same sequence.
        /// Used by: ManagedAccounts, AccountPositions, AccountSummary, TickSnapshot, ScannerParameters, FundamentalData.
        /// </summary>
        internal static IObservable<T> ToShareSource<T>(this IObservable<T> source)
        {
            var gate = new object();
            ReplaySubject<T> subject = new();
            IDisposable subjectSubscription = Disposable.Empty;

            return Observable.Create<T>(observer =>
            {
                lock (gate)
                {
                    if (subject.IsDisposed)
                        subject = new ReplaySubject<T>();

                    var observerSubscription = subject.Subscribe(observer);

                    if (subjectSubscription == Disposable.Empty)
                        subjectSubscription = source.Subscribe(subject);

                    return Disposable.Create(() =>
                    {
                        lock (gate)
                        {
                            observerSubscription.Dispose();
                            if (!subject.HasObservers)
                            {
                                subjectSubscription.Dispose();
                                subjectSubscription = Disposable.Empty;
                                subject.Dispose();
                            }
                        }
                    });
                }
            });
        }
    }
}
