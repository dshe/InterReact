using Stringification;
using System.Diagnostics;
using System.Globalization;

namespace InterReact;

internal static class ResponseExtensions
{
    internal static IObservable<object> ComposeMessages(this IObservable<string[]> source, Connection connection, ILoggerFactory loggerFactory)
    {
        ResponseMessageComposer composer = new(connection, loggerFactory);

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

    internal static IObservable<object> LogMessages(this IObservable<object> source, ILoggerFactory loggerFactory)
    {
        ILogger logger = loggerFactory.CreateLogger("InterReact.Messages");

        Stringifier stringifier = new(logger);

        return source.Do(msg =>
        {
            LogLevel logLevel = msg is AlertMessage ? LogLevel.Information : LogLevel.Debug;
            if (logger.IsEnabled(logLevel))
                logger.Log(logLevel, "Response {Message}", stringifier.Stringify(msg));
        });
    }
}
