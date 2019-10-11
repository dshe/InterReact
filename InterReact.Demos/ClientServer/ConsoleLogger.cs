using System;
using System.Reactive.Disposables;
using Microsoft.Extensions.Logging;
using RxSockets;

#nullable enable

namespace CoreClientServer
{
    public class MyLogger : ILogger
    {
        private readonly string CategoryName;
        private readonly ConsoleColor Color;

        public MyLogger(string categoryName, ConsoleColor color)
        {
            CategoryName = categoryName;
            Color = color;
        }
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            var color = Color;
            if (logLevel <= LogLevel.Debug)
                return;
            var oldColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine(CategoryName + formatter(state, exception));
            Console.ForegroundColor = oldColor;
        }
        public bool IsEnabled(LogLevel logLevel) => true;
        public IDisposable BeginScope<TState>(TState state) => Disposable.Empty;
    }

    public class MyLoggerFactory : ILoggerFactory
    {
        private readonly string? Name;
        private readonly ConsoleColor Color;

        public MyLoggerFactory(string? name = null, ConsoleColor color = ConsoleColor.Gray)
        {
            Name = name;
            Color = color;
        }

        public void AddProvider(ILoggerProvider provider) => throw new NotImplementedException();

        public ILogger CreateLogger(string name = "") => new MyLogger(Name ?? name, Color);

        public void Dispose() { }
    }
}
