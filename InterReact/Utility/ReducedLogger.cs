using Microsoft.Extensions.Logging;

namespace InterReact;

internal class ReducedLogger : ILogger
{
    private readonly ILogger YourLogger;
    internal ReducedLogger(ILogger logger) => YourLogger = logger;

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull =>
        YourLogger.BeginScope(state);

    public bool IsEnabled(LogLevel logLevel) =>
        YourLogger.IsEnabled(logLevel);

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        if (logLevel > 0)
            logLevel--; // reduce
        Log(logLevel, eventId, state, exception, formatter);
    }
}

public static partial class Extensionz
{
    internal static ILogger Reduce(this ILogger logger) => new ReducedLogger(logger);
}
