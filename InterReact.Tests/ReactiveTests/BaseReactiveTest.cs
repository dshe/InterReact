using Microsoft.Extensions.Logging;
using Microsoft.Reactive.Testing;
using System;
using Xunit;
using Xunit.Abstractions;

namespace InterReact.UnitTests
{
    [Trait("Category", "UnitReactiveTests")]
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
