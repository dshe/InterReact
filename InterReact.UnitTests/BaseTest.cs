using Microsoft.Extensions.Logging;
using Xunit.Abstractions;
using MXLogger;
using System;

namespace InterReact.UnitTests
{
    public abstract class BaseTest
    {
        protected readonly Action<string> Write;
        protected readonly ILoggerFactory LoggerFactory;
        protected readonly ILogger Logger;

        public BaseTest(ITestOutputHelper output)
        {
            Write = output.WriteLine;
            LoggerFactory = new LoggerFactory().AddMXLogger(Write);
            Logger = LoggerFactory.CreateLogger("Test");
        }
    }
}
