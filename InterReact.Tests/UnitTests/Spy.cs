using Stringification;
using System;
using System.Reactive.Linq;

namespace InterReact.UnitTests;

public static class SpyEx
{
    public static IObservable<T> Spy<T>(this IObservable<T> source,
        Action<string> write, string name = "")
    {
        Write("Create.");

        return Observable.Create<T>(observer =>
        {
            Write("Subscribe.");

            var sub = source.Subscribe(

                onNext: message =>
                {
                    Write("OnNext: " + message?.Stringify());
                    observer.OnNext(message);
                },
                onError: ex =>
                {
                    Write("OnError: " + ex.Message);
                    write(ex.ToString());
                    observer.OnError(ex);
                },
                onCompleted: () =>
                {
                    Write("OnCompleted.");
                    observer.OnCompleted();
                });

            return () =>
            {
                Write("Unsubscribe.");
                sub.Dispose();
            };
        });

        // local method
        void Write(string operation) =>
            write($"{Environment.CurrentManagedThreadId:D2}{name} {operation}");
    }

}
