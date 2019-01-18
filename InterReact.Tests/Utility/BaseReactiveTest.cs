using System;
using Microsoft.Reactive.Testing;
using Xunit.Abstractions;
using Xunit;

namespace InterReact.Tests.Utility
{
    [Trait("Category", "UnitTest")]
    public class BaseReactiveTest : ReactiveTest
    {
        protected readonly Action<string> Write;

        public BaseReactiveTest(ITestOutputHelper output) => Write = output.WriteLine;
    }
}
