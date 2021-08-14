using System;
using System.Reactive.Disposables;
using Microsoft.Extensions.Logging;

namespace CoreClientServer
{
    public class ConsoleLogger : ILogger
    {
        private readonly string CategoryName;
        private readonly ConsoleColor Color;

        public ConsoleLogger(string categoryName, ConsoleColor color)
        {
            CategoryName = categoryName;
            Color = color;
        }
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (logLevel <= LogLevel.Debug)
                return;
            ConsoleColor color = Color;
            ConsoleColor oldColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine(CategoryName + formatter(state, exception));
            Console.ForegroundColor = oldColor;
        }
        public bool IsEnabled(LogLevel logLevel) => true;
        public IDisposable BeginScope<TState>(TState state) => Disposable.Empty;
    }

}
