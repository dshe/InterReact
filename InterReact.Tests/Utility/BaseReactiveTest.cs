using System;
using Microsoft.Reactive.Testing;
using Xunit.Abstractions;
using Xunit;
using Microsoft.Extensions.Logging;
using MXLogger;

namespace InterReact.Tests.Utility
{
    [Trait("Category", "UnitTest")]
    public class BaseReactiveTest : ReactiveTest
    {
        protected readonly ILoggerFactory LoggerFactory;
        protected readonly ILogger Logger;

        public BaseReactiveTest(ITestOutputHelper output)
        {
            var provider = new MXLoggerProvider(output.WriteLine, LogLevel.Trace);
            LoggerFactory = new LoggerFactory(new[] { provider });
            Logger = LoggerFactory.CreateLogger("ReactiveTest");
        }

    }
}
