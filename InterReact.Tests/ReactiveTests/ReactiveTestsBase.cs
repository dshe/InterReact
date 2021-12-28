using Microsoft.Extensions.Logging;
using Microsoft.Reactive.Testing;
using System;
using Xunit;
using Xunit.Abstractions;

namespace InterReact.UnitTests;

[Trait("Category", "UnitReactiveTests")]
public class ReactiveTestsBase : ReactiveTest
{
    protected readonly Action<string> Write;
    protected readonly ILogger Logger;

    public ReactiveTestsBase(ITestOutputHelper output)
    {
        Write = output.WriteLine;
        Logger = new LoggerFactory()
            .AddMXLogger(Write, LogLevel.Debug)
            .CreateLogger("ReactiveTest");
    }
}
