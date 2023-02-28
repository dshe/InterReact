namespace InterReact;

public static partial class Extension
{
    public static ILoggerFactory ToLoggerFactory(this ILogger logger) => new FactoryLogger(logger);
}

internal sealed class FactoryLogger : ILoggerFactory
{
    private readonly ILogger Logger;
    internal FactoryLogger(ILogger logger) => Logger = logger;
    public void AddProvider(ILoggerProvider provider) => throw new NotImplementedException();
    public ILogger CreateLogger(string categoryName) => Logger;
    public void Dispose() { }
}

