using Microsoft.Extensions.Logging;

namespace UnitTests;

public abstract class UnitTestBase
{
    protected readonly Action<string> Write;
    protected readonly ILoggerFactory LoggerFactory;
    protected readonly ILogger Logger;

    public UnitTestBase(ITestOutputHelper output)
    {
        Write = output.WriteLine;

        LoggerFactory = new LoggerFactory()
            .AddMXLogger(Write, LogLevel.Debug);
        
        Logger = LoggerFactory
            .CreateLogger("Test");
    }
}
