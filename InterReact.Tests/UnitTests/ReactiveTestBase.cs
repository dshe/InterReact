using Microsoft.Extensions.Logging;
using Microsoft.Reactive.Testing;

namespace UnitTests;

public class ReactiveUnitTestBase : ReactiveTest
{
    protected readonly Action<string> Write;
    protected readonly ILoggerFactory LoggerFactory;
    protected readonly ILogger Logger;
    protected readonly TestScheduler testScheduler = new();
    protected int start = 0, stop = 0;

    public ReactiveUnitTestBase(ITestOutputHelper output)
    {
        Write = output.WriteLine;

        LoggerFactory = new LoggerFactory()
            .AddMXLogger(Write, LogLevel.Debug);

        Logger = LoggerFactory
            .CreateLogger("ReactiveTest");
    }
}
