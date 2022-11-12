using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace InterReact;

public static partial class Ext
{
    /// <summary>
    /// Returns an observable which shares a subscription to the source observable.
    /// Messages are relayed from the underlying sequence so that
    /// concurrent observers receive the same sequence.
    /// </summary>
    internal static IObservable<T> ShareSource<T>(this IObservable<T> source)
    {
        object gate = new();
        ReplaySubject<T>? subject = null;
        IDisposable subjectSubscription = Disposable.Empty;

        return Observable.Create<T>(observer =>
        {
            lock (gate)
            {
                subject ??= new();

                IDisposable observerSubscription = subject.Subscribe(observer);

                if (subjectSubscription == Disposable.Empty)
                    subjectSubscription = source.Subscribe(subject);

                return Disposable.Create(() =>
                {
                    lock (gate)
                    {
                        observerSubscription.Dispose();
                        if (subject is null || subject.HasObservers)
                            return;
                        subjectSubscription.Dispose();
                        subjectSubscription = Disposable.Empty;
                        subject.Dispose();
                        subject = null;
                    }
                });
            }
        });
    }
}
