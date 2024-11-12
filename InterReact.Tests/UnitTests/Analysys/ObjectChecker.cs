using System.Reflection;
namespace Analysis;

public class Interface_Checker(ITestOutputHelper output) : UnitTestBase(output)
{
    private static readonly List<TypeInfo> Types = typeof(InterReactClient)
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
                t != typeof(OrderMonitor) &&
                t.GetProperty("OrderId") is not null &&
                t.GetInterface("IHasOrderId") is null));
    }

    [Fact]
    public void Find_Classes_With_ExecutionId_But_Not_Interface()
    {
        Assert.Empty(Types.Where(t =>
                t.GetProperty("ExecutionId") is not null &&
                t.GetInterface("IHasExecutionId") is null));
    }

    [Fact]
    public void Find_Classes_With_Tick_But_Not_Interface()
    {
        Assert.Empty(Types.Where(t =>
                t.GetProperty("TickType") is not null &&
                t.GetInterface("ITick") is null));
    }

    [Fact]
    public void Find_Classes_With_TickByTick_But_Not_Interface()
    {
        Assert.Empty(Types.Where(t =>
                t.GetProperty("TickByTickType") is not null &&
                t.GetInterface("ITickByTick") is null));
    }
}
