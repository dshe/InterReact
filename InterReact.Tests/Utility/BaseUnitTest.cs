using System;
using Xunit;
using Xunit.Abstractions;

namespace InterReact.Tests.Utility
{
    [Trait("Category", "UnitTest")]
    public class BaseUnitTest
    {
        protected readonly Action<string> Write;
        public BaseUnitTest(ITestOutputHelper output) => Write = output.WriteLine;
    }

}
