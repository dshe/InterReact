using Microsoft.Extensions.Logging;

namespace ConnectTests;

public abstract class ConnectTestBase
{
    protected readonly ILoggerFactory LogFactory;
    protected readonly Action<string> Write;

    protected ConnectTestBase(ITestOutputHelper output, LogLevel logLevel = LogLevel.Debug)
    {
        LogFactory = LoggerFactory
            .Create(builder => builder
                .AddMXLogger(output.WriteLine)
                .SetMinimumLevel(logLevel));
        
        Write = s => output.WriteLine(s + "\r\n");
    }
}
