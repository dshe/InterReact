using System;
using System.Reactive.Linq;
using InterReact.Extensions;
using Microsoft.Extensions.Logging;

namespace InterReact.Tests.Utility
{
    public static class SpyEx
    {
        public static IObservable<T> Spy<T>(this IObservable<T> source, ILogger logger, string name = "")
        {
            Write("Create.");

            return Observable.Create<T>(observer =>
            {
                Write("Subscribe.");

                var sub = source.Subscribe(
                    onNext: message =>
                    {
                        if (message == null)
                            Write("OnNext: null.");
                        else
                            Write("OnNext: " + message.Stringify());
                        observer.OnNext(message);
                    },
                    onError: ex =>
                    {
                        Write("");
                        Write("OnError: " + ex.Message);
                        Write("");
                        Write(ex.ToString());
                        Write("");
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

            // local
            void Write(string operation) =>
                logger.LogDebug($"{Environment.CurrentManagedThreadId:D2}{name} {operation}");
        }

    }

}
