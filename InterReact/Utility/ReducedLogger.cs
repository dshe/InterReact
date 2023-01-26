using Microsoft.Extensions.Logging;

namespace InterReact;

internal sealed class ReducedLogger : ILogger
{
    private readonly ILogger Logger;
    internal ReducedLogger(ILogger logger) => Logger = logger;

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull =>
        Logger.BeginScope(state);

    public bool IsEnabled(LogLevel logLevel) =>
        Logger.IsEnabled(logLevel);

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        if (logLevel == LogLevel.Trace || logLevel == LogLevel.None)
            return;
        logLevel--; // reduce
        Log(logLevel, eventId, state, exception, formatter);
    }
}

public static partial class Extension
{
    internal static ILogger Reduce(this ILogger logger) => new ReducedLogger(logger);
}
