using System;
using Microsoft.Reactive.Testing;
using Xunit.Abstractions;
using Xunit;
using Microsoft.Extensions.Logging;

namespace InterReact.Tests.Utility
{
    [Trait("Category", "UnitTest")]
    public class BaseReactiveTest : ReactiveTest
    {
        protected readonly ILogger Logger;

        public BaseReactiveTest(ITestOutputHelper output)
        {
            Logger = output.BuildLoggerFor<IInterReactClient>(); // Divergic.Logging.Xunit
        }

    }
}
