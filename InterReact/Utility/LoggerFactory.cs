using Microsoft.Extensions.Logging;

namespace InterReact.Utility
{
    internal static class LoggerFactoryProviderExtensions
    {
        // ILoggingBuilder is defined in Microsoft.Extensions.Logging
        internal static void AddFactory(this ILoggingBuilder builder, ILoggerFactory loggerFactory)
        {
            var provider = new MyLoggerProvidor(loggerFactory);
            builder.AddProvider(provider);
        }
    }

    public class MyLoggerProvidor : ILoggerProvider
    {
        private readonly ILoggerFactory LoggerFactory;
        public MyLoggerProvidor(ILoggerFactory factory)
        {
            LoggerFactory = factory;
        }

        public ILogger CreateLogger(string categoryName)
        {
            return LoggerFactory.CreateLogger(categoryName);
        }

        public void Dispose() { }
    }
}
