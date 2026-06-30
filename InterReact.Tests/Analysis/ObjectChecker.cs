using System.Reflection;
namespace Analysis;

public class Interface_Checker(ITestOutputHelper output) : OutputHelperTestBase(output)
{
    private static TypeInfo[] GetTypes(Func<TypeInfo, bool> predicate) =>
        [.. typeof(InterReactClient).Assembly.DefinedTypes.Where(predicate).OrderBy(t => t.Name)];

    private static T[] SymmetricExcept<T>(IEnumerable<T> first, IEnumerable<T> second) =>
        [.. first.Except(second).Union(second.Except(first))];

    [Fact]
    public void Find_Classes_With_RequestId()
    {
        TypeInfo[] types1 = GetTypes(t => t.GetInterface("IHasRequestId") is not null);
        Assert.Empty(types1.Where(t => t.GetCustomAttribute<MessageAttribute>() == null && !t.IsAbstract));

        TypeInfo[] types2 = GetTypes(t => t.GetProperty("RequestId") is not null && !t.IsInterface);
        TypeInfo[] except = SymmetricExcept(types1, types2);

        Assert.Empty(except);

        foreach (Type t in types1)
            Write(t.FullName!);
    }

    [Fact]
    public void Find_Classes_With_OrderId()
    {
        TypeInfo[] types1 = GetTypes(t => t.GetInterface("IHasOrderId") is not null);
        Assert.Empty(types1.Where(t => t.GetCustomAttribute<MessageAttribute>() == null && !t.IsAbstract));

        TypeInfo[] types2 = GetTypes(t => t.GetProperty("OrderId") is not null && !t.IsInterface);
        TypeInfo[] except = SymmetricExcept(types1, types2);
        Assert.Empty(except.Where(t => t.Name != "OrderMonitor"));

        foreach (Type t in types1)
            Write(t.FullName!);
    }

    [Fact]
    public void Find_Classes_With_ExecutionId()
    {
        TypeInfo[] types1 = GetTypes(t => t.GetInterface("IHasExecutionId") is not null);
        Assert.Empty(types1.Where(t => t.GetCustomAttribute<MessageAttribute>() == null && !t.IsAbstract));

        TypeInfo[] types2 = GetTypes(t => t.GetProperty("ExecutionId") is not null && !t.IsInterface);
        TypeInfo[] except = SymmetricExcept(types1, types2);
        Assert.Empty(except);

        foreach (Type t in types1)
            Write(t.FullName!);
    }

    [Fact]
    public void Find_Classes_With_Code()
    {
        TypeInfo[] types1 = GetTypes(t => t.GetInterface("IHasStringCode") is not null);
        Assert.Empty(types1.Where(t => t.GetCustomAttribute<MessageAttribute>() != null));

        TypeInfo[] types2 = GetTypes(t => t.GetProperty("StringCode") is not null && !t.IsInterface);
        TypeInfo[] except = SymmetricExcept(types1, types2);
        Assert.Empty(except);

        foreach (Type t in types1)
            Write(t.FullName!);
    }


}
