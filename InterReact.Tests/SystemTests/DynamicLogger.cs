using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace InterReact.SystemTests
{
    public class DynamicLogger : ILogger
    {
        private readonly object Locker = new();
        private ILogger Logger = NullLogger.Instance;
        public void Set(ILogger logger)
        {
            lock (Locker)
            {
                Logger = logger;
            }
        }
        public IDisposable BeginScope<TState>(TState state)
        {
            lock (Locker)
            {
                return Logger.BeginScope(state);
            }
        }
        public bool IsEnabled(LogLevel logLevel)
        {
            lock (Locker)
            {
                return Logger.IsEnabled(logLevel);
            }
        }
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            lock (Logger)
            {
                Logger.Log(logLevel, eventId, state, exception, formatter);
            }
        }
    }

    public class DynamicLoggers : ILogger
    {
        private readonly HashSet<ILogger> Loggers = new();
        public void Add(ILogger logger)
        {
            lock (Loggers)
            {
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
        public IDisposable BeginScope<TState>(TState state)
        {
            lock (Loggers)
            {
                var disposables = Loggers.Select(logger => logger.BeginScope(state));
                return new CompositeDisposable(disposables);
            }
        }
        public bool IsEnabled(LogLevel logLevel)
        {
            lock (Loggers)
            {
                return Loggers.Any() && Loggers.First().IsEnabled(logLevel);
            }
        }
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            lock (Loggers)
            {
                foreach (var logger in Loggers)
                    logger.Log(logLevel, eventId, state, exception, formatter);
            }
        }
    }

}
