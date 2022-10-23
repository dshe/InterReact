using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xunit;
using Xunit.Abstractions;

namespace InterReact.UnitTests.Analysis;

public class Interface_Checker : UnitTestsBase
{
    public Interface_Checker(ITestOutputHelper output) : base(output) { }

    private static readonly List<TypeInfo> Types =
        typeof(InterReactClient)
        .Assembly
        .DefinedTypes.Where(t => t.IsClass)
        .ToList();

    [Fact]
    public void Find_Classes_With_RequestId_But_Not_Interface()
    {
        Assert.Empty(Types.Where(t =>
                t.GetProperty("RequestId") is not null &&
                t.GetInterface("IHasRequestId") is null));
    }

    [Fact]
    public void Find_Classes_With_OrderId_But_Not_Interface()
    {
        Assert.Empty(Types.Where(t =>
                t.GetProperty("OrderId") is not null &&
                t.GetInterface("IHasOrderId") is null));
    }

    [Fact]
    public void Find_Classes_With_ExecutioinId_But_Not_Interface()
    {
        Assert.Empty(Types.Where(t =>
                t.GetProperty("ExecutionId") is not null &&
                t.GetInterface("IHasExecutionId") is null));
    }
}
