using Microsoft.Extensions.Logging;
using System;
using Xunit;
using Xunit.Abstractions;

namespace InterReact.UnitTests
{
    [Trait("Category", "UnitTests")]
    public abstract class BaseUnitTest
    {
        protected readonly Action<string> Write;
        protected readonly ILoggerFactory LoggerFactory;
        protected readonly ILogger Logger;

        public BaseUnitTest(ITestOutputHelper output)
        {
            Write = output.WriteLine;
            LoggerFactory = new LoggerFactory().AddMXLogger(Write);
            Logger = LoggerFactory.CreateLogger("Test");
        }
    }
}
