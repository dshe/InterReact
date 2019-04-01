using Microsoft.Extensions.Logging;
using System;
using Xunit;
using Xunit.Abstractions;

namespace InterReact.Tests.Utility
{
    [Trait("Category", "UnitTest")]
    public abstract class BaseUnitTest
    {
        protected readonly ILogger Logger;

        public BaseUnitTest(ITestOutputHelper output)
        {
            Logger = output.BuildLoggerFor<IInterReactClient>(); // Divergic.Logging.Xunit
        }
    }

}
