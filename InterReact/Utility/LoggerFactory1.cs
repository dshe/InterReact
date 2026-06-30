namespace InterReact;

public static partial class Xtensions
{
    extension(ILogger logger)
    {
        internal ILoggerFactory ToLoggerFactory() => new LoggerFactory1(logger);
    }
}

file sealed class LoggerFactory1 : ILoggerFactory
{
    private readonly ILogger _logger;
    internal LoggerFactory1(ILogger logger) => _logger = logger;

    public void AddProvider(ILoggerProvider provider) => throw new NotImplementedException();

    /// <summary>
    /// Simply returns the logger passed in the constructor.
    /// Does not actually create a logger of categoryName.
    /// </summary>
    /// <param name="ignoredCategoryName"></param>
    /// <returns></returns>
    public ILogger CreateLogger(string ignoredCategoryName = "") => _logger;

    public void Dispose() { }
}
