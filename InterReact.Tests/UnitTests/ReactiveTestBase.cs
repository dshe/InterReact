using Microsoft.Extensions.Logging;
using Microsoft.Reactive.Testing;

namespace UnitTests;

public abstract class ReactiveUnitTestBase : ReactiveTest
{
    private readonly ITestOutputHelper Output;
    protected readonly ILoggerFactory LogFactory;
    protected readonly ILogger Logger;
    protected void Write(string format, params object[] args) =>
        Output.WriteLine(string.Format(format, args) + Environment.NewLine);

    protected ReactiveUnitTestBase(ITestOutputHelper output, LogLevel logLevel = LogLevel.Trace, string name = "Test")
    {
        Output = output;
        LogFactory = LoggerFactory.Create(builder => builder
            .AddMXLogger(output.WriteLine)
            .SetMinimumLevel(logLevel));
        Logger = LogFactory.CreateLogger(name);
    }
}
