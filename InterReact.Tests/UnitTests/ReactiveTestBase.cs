using Microsoft.Extensions.Logging;
using Microsoft.Reactive.Testing;

namespace UnitTests;

public class ReactiveTestBase : ReactiveTest
{
    protected readonly Action<string> Write;
    protected readonly ILogger Logger;

    public ReactiveTestBase(ITestOutputHelper output)
    {
        Write = output.WriteLine;

        Logger = new LoggerFactory()
            .AddMXLogger(Write, LogLevel.Debug)
            .CreateLogger("ReactiveTest");
    }
}
