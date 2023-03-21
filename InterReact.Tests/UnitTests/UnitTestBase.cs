using Microsoft.Extensions.Logging;

namespace UnitTests;

public abstract class UnitTestBase
{
    protected readonly ILogger Logger;
    protected readonly Action<string> Write;

    protected UnitTestBase(ITestOutputHelper output, LogLevel logLevel = LogLevel.Debug)
    {
        Logger = LoggerFactory
            .Create(builder => builder
                .AddMXLogger(output.WriteLine)
                .SetMinimumLevel(logLevel))
            .CreateLogger("UnitTest");

        Write = (s) => output.WriteLine(s + "\r\n");
    }
}
