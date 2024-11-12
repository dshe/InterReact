using Stringification;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
namespace Analysis;

public class Types_Viewer(ITestOutputHelper output) : UnitTestBase(output)
{
    private static readonly List<TypeInfo> Types = typeof(InterReactClient)
        .Assembly
        .DefinedTypes
        .Where(t => !t.Name.Contains("<>"))
        .ToList();

    [Fact]
    public void List_Type_Attributes()
    {
        foreach (var group in Types
            .Select(ti => ti.GetCustomAttributes(true).Select(a => new { a, ti }))
            .SelectMany(x => x)
            .Where(x => x.a is not (CompilerGeneratedAttribute or ExtensionAttribute))
            .GroupBy(x => x.a))
        {
            Write(group.Key.ToString()!);
            foreach (var a in group.OrderBy(x => x.ti?.FullName))
                Write($"    {a.ti?.FullName}");
        }
    }

    [Fact]
    public void List_Member_Attributes()
    {
        foreach (var group in Types
            .Select(t => t.DeclaredMembers
                .Where(m => !m.Name.StartsWith('<'))
                .Select(m => (t, m)))
            .SelectMany(x => x)
            //.Where(x => x.m is not null) //.OfType<(TypeInfo, MemberInfo)>()

            .Select(x => x.m.GetCustomAttributes(false)
                .Select(q => new { type = x.t, method = x.m, attr = q }))
            .SelectMany(x => x)
            .Where(x => x.attr is not (
                CompilerGeneratedAttribute or
                DebuggerHiddenAttribute or
                SecuritySafeCriticalAttribute or
                AsyncStateMachineAttribute or
                DebuggerStepThroughAttribute))
            .GroupBy(x => x.attr))
        {
            Write(group.Key.ToString()!);
            foreach (var a in group.OrderBy(x => x.type?.FullName + x.method?.Name))
                Write($"     {a.type?.FullName}  {a.method?.Name}");
        }
    }

    [Fact]
    public void Auto_Type_And_Stringify_One()
    {
        Stringifier s = new(Logger);
        var tick = new PriceTick(0, TickType.Undefined, 0, new TickAttrib());
        Write(s.Stringify(tick));

        //TypeInfo type = typeof(PriceTick).GetTypeInfo();
        //object instance = s.CreateInstance(type);
        //Assert.NotNull(instance);
        //Write(s.Stringify(instance));
    }

    [Fact]
    public void Auto_Type_And_Stringify_All() // sometimes fails?
    {
        IEnumerable<TypeInfo> types = Types
            .Where(t =>
                t is
                {
                    IsClass: true,
                    IsPublic: true,
                    IsSealed: true,
                    IsAbstract: false,
                    ContainsGenericParameters: false,
                    Namespace: "InterReact"
                })
            .OrderBy(x => x.Name);            ;

        foreach (TypeInfo type in types)
        {
            if (type == typeof(OrderMonitor) ||
                type == typeof(TickEnumerableSelector) ||
                type == typeof(TickObservableSelector) ||
                type == typeof(InterReactOptions) ||
                type == typeof(NullInterReactClient) ||
                type == typeof(InterReactClient))
                continue;
            try
            {
                object instance = Stringifier.Instance.CreateInstance(type);
                Assert.NotNull(instance);
                string str = Stringifier.Instance.Stringify(instance);
                Write($"Type: {str}");
            }
            catch (Exception e)
            {
                Write($"Type: {type.Name} EXCEPTION: {e.Message}");
                throw;
            }
        }
    }
}
