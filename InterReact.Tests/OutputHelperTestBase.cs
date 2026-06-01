using Microsoft.Extensions.Logging;
using Microsoft.Reactive.Testing;
namespace Tests;

public abstract class OutputHelperTestBase : ReactiveTest
{
    private readonly ITestOutputHelper _output;
    protected readonly ILoggerFactory LogFactory;
    protected readonly ILogger Logger;
    protected void Write(string format, params object[] args) =>
        _output.WriteLine(string.Format(format, args) + Environment.NewLine);
    protected void Write(string str) => _output.WriteLine(str);

    protected OutputHelperTestBase(ITestOutputHelper output, LogLevel logLevel = LogLevel.Debug, string name = "Test")
    {
        _output = output;
        LogFactory = LoggerFactory.Create(builder => builder
            .AddMXLogger(output.WriteLine)
            .SetMinimumLevel(logLevel));
        Logger = LogFactory.CreateLogger(name);
    }
}
