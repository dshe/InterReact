using Microsoft.Extensions.Logging;
using Stringification;
using System.Reactive.Linq;

namespace InterReact;

internal static class ResponseExtensions
{
    internal static IObservable<object> ComposeMessages(this IObservable<string[]> source, InterReactClientConnector connector)
    {
        ResponseMessageComposer composer = new(connector);
        return source.Select(strings => composer.ComposeMessage(strings));
    }

    internal static IObservable<object> FollowPriceTickWithSizeTick(this IObservable<object> source, bool followPriceTickWithSizeTick)
    {
        if (!followPriceTickWithSizeTick)
            return source;

        return Observable.Create<object>(observer =>
        {
            return source.Subscribe(
                message =>
                {
                    observer.OnNext(message);
                    if (message is not PriceTick priceTick)
                        return;
                    TickType type = priceTick.GetSizeTickType();
                    if (type == TickType.Undefined)
                        return;
                    SizeTick sizeTick = new()
                    {
                        RequestId = priceTick.RequestId,
                        TickType = type,
                        Size = priceTick.Size
                    };
                    observer.OnNext(sizeTick);
                },
                observer.OnError,
                observer.OnCompleted);
        });
    }

    internal static IObservable<object> LogMessages(this IObservable<object> source, ILogger logger)
    {
        Stringifier stringifier = new(logger);
        return source.Do(msg =>
        {
            LogLevel logLevel = msg is Alert ? LogLevel.Information : LogLevel.Debug;
            logger.Log(logLevel, "Msg: {Message}", stringifier.Stringify(msg));
        });
    }
}

