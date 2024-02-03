using Stringification;
using System.Diagnostics;

namespace InterReact;

internal static class ResponseExtensions
{
    internal static IObservable<object> ComposeMessages(this IObservable<string[]> source, ResponseMessageComposer composer)
    {
        return Observable.Create<object>(observer =>
        {
            return source.Subscribe(
                message =>
                {
                    try
                    {
                        object result = composer.ComposeMessage(message);
                        if (result is object[] results)
                        {
                            Debug.Assert(message[0] == "1");
                            Debug.Assert(results.Length == 2);
                            observer.OnNext(results[0]); // priceTick
                            observer.OnNext(results[1]); // sizeTick
                            return;
                        }
                        observer.OnNext(result);
                    }
#pragma warning disable CA1031
                    catch (Exception e)
#pragma warning restore CA1031
                    {
                        observer.OnError(e);
                    }
                },
                observer.OnError,
                observer.OnCompleted);
        });
    }

    internal static IObservable<object> LogMessages(this IObservable<object> source, ILogger logger, Stringifier stringifier)
    {
        return source.Do(msg =>
        {
            LogLevel logLevel = msg is AlertMessage ? LogLevel.Information : LogLevel.Debug;
            if (logger.IsEnabled(logLevel))
                logger.Log(logLevel, "Response {Message}", stringifier.Stringify(msg));
        });
    }
}
