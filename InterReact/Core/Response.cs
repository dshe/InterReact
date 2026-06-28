using System.Diagnostics;
namespace InterReact;

public sealed class Response : IObservable<object>
{
    private readonly IObservable<object> _observable;

    public Response(Connection connection, ResponseComposer responseComposer, ILogger<Response> logger)
    {
        ArgumentNullException.ThrowIfNull(connection, nameof(connection));

        _observable = connection
            .Observable
            .ComposeMessage(responseComposer, logger)
            .Publish()
            .AutoConnect(); // connect on first observer
    }

    public IDisposable Subscribe(IObserver<object> observer) => _observable.Subscribe(observer);
}

public static class Xtension
{
    extension(IObservable<string[]> source)
    {
        internal IObservable<object> ComposeMessage(ResponseComposer responseComposer, ILogger logger)
        {
            return Observable.Create<object>(observer =>
            {
                return source.Subscribe(onNext: strings =>
                {
                    try
                    {
                        object message = responseComposer.Compose(strings);
                        if (message is object[] messages)
                        {
                            Debug.Assert(strings[0] == "1");
                            Debug.Assert(messages.Length == 2);
                            if (logger.IsEnabled(LogLevel.Debug))
                                logger.LogResponseMessage(messages[0].Stringify());
                            observer.OnNext(messages[0]); // priceTick
                            if (logger.IsEnabled(LogLevel.Debug))
                                logger.LogResponseMessage(messages[1].Stringify());
                            observer.OnNext(messages[1]); // sizeTick
                            return;
                        }
                        if (logger.IsEnabled(LogLevel.Debug))
                            logger.LogResponseMessage(message.Stringify());
                        observer.OnNext(message);
                    }
                    catch (Exception ex)
                    {
                        string msg = $"Response({strings[0]}): {ex.Message}.";
                        logger.LogError(ex, "{Msg}", msg);
                        Alert alert = new() { Message = msg };
                        observer.OnNext(alert);
                        throw new InvalidOperationException(msg, ex);
                    }
                },
                onError: observer.OnError,
                onCompleted: observer.OnCompleted);
            });
        }
    }
}
