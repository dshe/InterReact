namespace InterReact;

public static partial class Xtension
{
    extension(IObservable<object> source)
    {
        internal IObservable<object> Log(ILogger logger)
        {
            return source.Do(
            m =>
            {
                if (logger.IsEnabled(LogLevel.Debug))
                    logger.LogResponseMessage(m.Stringify());
            },
            ex => logger.LogError(ex, "Error."),
            () => logger.LogInformation("OnCompleted."));
        }
    }
}
