using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using InterReact;
using InterReact.Tests.Utility;
using Xunit;
using Xunit.Abstractions;

namespace InterReact.Tests.UnitTests.Analysis
{
    public class ObjectChecker : BaseUnitTest
    {
        private static readonly IEnumerable<TypeInfo> Types =
            typeof(InterReactClient).GetTypeInfo().Assembly
            .DefinedTypes.Where(t => t.IsClass);

        public ObjectChecker(ITestOutputHelper output) : base(output) {}

        [Fact]
        public void Find_Classes_With_RequestId_But_Not_Interface()
        {
            Assert.Empty(Types.Where(t =>
                    t.GetProperty("RequestId") != null &&
                    t.GetInterface("IHasRequestId") == null));
        }

        [Fact]
        public void Find_Classes_With_OrderId_But_Not_Interface()
        {
            Assert.Empty(Types.Where(t =>
                    t.GetProperty("OrderId") != null &&
                    t.GetInterface("IHasOrderId") == null));
        }

    }
}
