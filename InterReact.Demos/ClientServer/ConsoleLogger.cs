using System;
using System.Reactive.Disposables;
using Microsoft.Extensions.Logging;

namespace CoreClientServer;

public class ConsoleLogger : ILogger
{
    private readonly string CategoryName;
    private readonly LogLevel LogLevel;
    private readonly ConsoleColor Color;

    public ConsoleLogger(string categoryName, LogLevel logLevel, ConsoleColor color)
    {
        CategoryName = categoryName;
        LogLevel = logLevel;
        Color = color;
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        if (!IsEnabled(logLevel))
            return;
        ConsoleColor oldColor = Console.ForegroundColor;
        Console.ForegroundColor = Color;
        Console.WriteLine(CategoryName + formatter(state, exception));
        Console.ForegroundColor = oldColor;
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
