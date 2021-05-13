using Microsoft.Extensions.Logging;
using Microsoft.Reactive.Testing;
using Xunit.Abstractions;
using MXLogger;
using System;

namespace InterReact.UnitTests
{
    public class BaseReactiveTest : ReactiveTest
    {
        protected readonly Action<string> Write;
        protected readonly ILogger Logger;

        public BaseReactiveTest(ITestOutputHelper output)
        {
            Write = output.WriteLine;
            Logger = new LoggerFactory()
                .AddMXLogger(Write)
                .CreateLogger("ReactiveTest");
        }
    }
}
