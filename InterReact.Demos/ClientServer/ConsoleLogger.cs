using System.Reactive.Disposables;
using System.Threading;
namespace ClientServer;

public sealed class ConsoleLogger(
    string categoryName, LogLevel logLevel, ConsoleColor color) : ILogger
{
    private static Lock _gate = new();
    private readonly string _categoryName = categoryName;
    private readonly LogLevel _logLevel = logLevel;
    private readonly ConsoleColor _color = color;

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        if (!IsEnabled(logLevel))
            return;
        lock (_gate)
        {
            Console.ForegroundColor = _color;
            Console.WriteLine(_categoryName + formatter(state, exception));
        }
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        if (logLevel == LogLevel.None)
            return false;
        return logLevel >= _logLevel;
    }

    public IDisposable BeginScope<TState>(TState state) where TState : notnull
        => Disposable.Empty;
}
