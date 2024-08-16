using System.Reactive.Disposables;

namespace ClientServer;

public sealed class ConsoleLogger(
    string categoryName, LogLevel logLevel, ConsoleColor color) : ILogger
{
    private readonly string CategoryName = categoryName;
    private readonly LogLevel LogLevel = logLevel;
    private readonly ConsoleColor Color = color;

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        if (!IsEnabled(logLevel))
            return;
        Console.ForegroundColor = Color;
        Console.WriteLine(CategoryName + formatter(state, exception));
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        if (logLevel == LogLevel.None)
            return false;
        return logLevel >= LogLevel;
    }

    public IDisposable BeginScope<TState>(TState state) where TState : notnull
        => Disposable.Empty;
}
