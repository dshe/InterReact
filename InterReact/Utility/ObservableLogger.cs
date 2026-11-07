namespace InterReact;

public static partial class Xtension
{
    extension(IObservable<object> source)
    {
        internal IObservable<object> Log(ILogger logger, CancellationToken ct)
        {
            return source.Do(
            m =>
            {
                if (logger.IsEnabled(LogLevel.Debug))
                    logger.LogResponseMessage(m.Stringify());
            },
            ex =>
            {
                if (!ct.IsCancellationRequested)
                    logger.LogError(ex, "Error: {Exception}", ex.Message);
            },
            () => logger.LogInformation("OnCompleted."));
        }
    }
}
