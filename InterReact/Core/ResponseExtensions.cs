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
                    TickType type = GetSizeTickType(priceTick.TickType);
                    if (type == TickType.Undefined)
                        return;
                    SizeTick sizeTick = new(priceTick.RequestId, type, priceTick.Size);
                    observer.OnNext(sizeTick);
                },
                e => observer.OnError(e),
                observer.OnCompleted);
        });

        // local
        static TickType GetSizeTickType(TickType tickType) => tickType switch
        {
            TickType.BidPrice => TickType.BidSize,
            TickType.AskPrice => TickType.AskSize,
            TickType.LastPrice => TickType.LastSize,
            TickType.DelayedBidPrice => TickType.DelayedBidSize,
            TickType.DelayedAskPrice => TickType.DelayedAskSize,
            TickType.DelayedLastPrice => TickType.DelayedLastSize,
            _ => TickType.Undefined
        };
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

