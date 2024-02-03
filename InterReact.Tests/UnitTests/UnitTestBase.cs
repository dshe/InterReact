using Microsoft.Extensions.Logging;

namespace UnitTests;

public abstract class UnitTestBase
{
    private readonly ITestOutputHelper Output;
    protected readonly ILoggerFactory LogFactory;
    protected readonly ILogger Logger;
    protected void Write(string format, params object[] args) =>
        Output.WriteLine(string.Format(format, args) + Environment.NewLine);
    protected void Write(string str) =>
        Output.WriteLine(str+ Environment.NewLine);

    protected UnitTestBase(ITestOutputHelper output, LogLevel logLevel = LogLevel.Debug, string name = "Test")
    {
        Output = output;
        LogFactory = LoggerFactory.Create(builder => builder
            .AddMXLogger(output.WriteLine)
            .SetMinimumLevel(logLevel));
        Logger = LogFactory.CreateLogger(name);
    }
}
