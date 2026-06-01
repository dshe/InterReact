using System.Reflection;
namespace Analysis;

public class Interface_Checker(ITestOutputHelper output) : OutputHelperTestBase(output)
{
    private static readonly List<TypeInfo> _types = typeof(InterReactClient)
        .Assembly
        .DefinedTypes.Where(t => t.IsClass)
        .ToList();

    [Fact]
    public void Find_Classes_With_RequestId_But_Not_Interface()
    {
        Assert.Empty(_types.Where(t =>
                t.GetProperty("RequestId") is not null &&
                t.GetInterface("IHasRequestId") is null));
    }

    [Fact]
    public void Find_Classes_With_OrderId_But_Not_Interface()
    {
        Assert.Empty(_types.Where(t =>
                t != typeof(OrderMonitor) &&
                t.GetProperty("OrderId") is not null &&
                t.GetInterface("IHasOrderId") is null));
    }

    [Fact]
    public void Find_Classes_With_ExecutionId_But_Not_Interface()
    {
        Assert.Empty(_types.Where(t =>
                t.GetProperty("ExecutionId") is not null &&
                t.GetInterface("IHasExecutionId") is null));
    }
}
