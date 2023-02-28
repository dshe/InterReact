using Microsoft.Extensions.Logging;
using Microsoft.Reactive.Testing;

namespace UnitTests;

public abstract class ReactiveUnitTestBase : ReactiveTest
{
    protected readonly ILogger Logger;
    protected readonly Action<string> Write;

    protected ReactiveUnitTestBase(ITestOutputHelper output, LogLevel logLevel = LogLevel.Trace)
    {
        Logger = LoggerFactory
            .Create(builder => builder
                .AddMXLogger(output.WriteLine)
                .SetMinimumLevel(logLevel))
            .CreateLogger("ReactiveUnitTest");

        Write = (s) => output.WriteLine(s + "\r\n");
    }
}
