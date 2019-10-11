using Microsoft.Extensions.Logging;
using System;
using System.Text;
using Xunit;
using Xunit.Abstractions;
using MXLogger;

namespace InterReact.Tests.Utility
{
    [Trait("Category", "UnitTest")]
    public abstract class BaseUnitTest
    {
        protected readonly ILoggerFactory LoggerFactory;
        protected readonly ILogger Logger;

        public BaseUnitTest(ITestOutputHelper output)
        {
            var providor = new MXLoggerProvider(output.WriteLine, LogLevel.Trace);
            LoggerFactory = new LoggerFactory( new[] { providor });
            Logger = LoggerFactory.CreateLogger("UnitTest");
        }
    }

}
