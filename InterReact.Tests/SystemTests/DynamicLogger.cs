using Microsoft.Extensions.Logging;
using System.Reactive.Disposables;

namespace SystemTests;

public class DynamicLogger : ILogger
{
    private readonly HashSet<ILogger> Loggers = new();
    public void Add(ILogger logger, bool clear = false)
    {
        lock (Loggers)
        {
            if (clear)
                Loggers.Clear();
            if (!Loggers.Add(logger))
                throw new InvalidOperationException("Add");
        }
    }
    public void Remove(ILogger logger)
    {
        lock (Loggers)
        {
            if (!Loggers.Remove(logger))
                throw new InvalidOperationException("Remove");
        }
    }

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
    {
        lock (Loggers)
        {
            IEnumerable<IDisposable> disposables = Loggers
                .Select(logger => logger.BeginScope(state))
                .OfType<IDisposable>();

            return new CompositeDisposable(disposables);
        }
    }
    public bool IsEnabled(LogLevel logLevel)
    {
        lock (Loggers)
        {
            return Loggers
                .Where(l => l.IsEnabled(logLevel))
                .Any();
        }
    }
    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        lock (Loggers)
        {
            foreach (ILogger logger in Loggers)
                logger.Log(logLevel, eventId, state, exception, formatter);
        }
    }
}
