using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace InterReact
{
    public static partial class Extensions
    {
        /// <summary>
        /// Returns an observable which shares a subscription to the source observable.
        /// Messages are relayed from the underlying sequence so that,
        /// in the case of concurrent observers, all receive the same sequence.
        /// </summary>
        internal static IObservable<T> ShareSource<T>(this IObservable<T> source)
        {
            object gate = new();
            ReplaySubject<T> replaySubject = new();
            IDisposable subjectSubscription = Disposable.Empty;

            return Observable.Create<T>(observer =>
            {
                lock (gate)
                {
                    if (replaySubject.IsDisposed)
                        replaySubject = new ReplaySubject<T>();

                    IDisposable observerSubscription = replaySubject.Subscribe(observer);

                    if (subjectSubscription == Disposable.Empty)
                        subjectSubscription = source.Subscribe(replaySubject);

                    return Disposable.Create(() =>
                    {
                        lock (gate)
                        {
                            observerSubscription.Dispose();
                            if (replaySubject.HasObservers)
                                return;
                            subjectSubscription.Dispose();
                            subjectSubscription = Disposable.Empty;
                            replaySubject.Dispose();
                        }
                    });
                }
            });
        }
    }
}
