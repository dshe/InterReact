using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
namespace Analysis;

public class Types_Viewer(ITestOutputHelper output) : OutputHelperTestBase(output)
{
    private static readonly Assembly _assembly = typeof(InterReactClient).Assembly;

    private static readonly List<TypeInfo> _types = typeof(InterReactClient)
        .Assembly
        .DefinedTypes
        .Where(t => !t.Name.Contains("<>"))
        .ToList();

    [Fact]
    public void List_Namespaces()
    {
        List<Type> types = _assembly
            .GetTypes()
            .Where(t => t.IsClass)
            .Where(t => t.Namespace != null)
            .DistinctBy(t => t.Namespace)
            .OrderBy(x => x.Name)
            .ToList();

        foreach (TypeInfo type in types)
            Write($"Namespace: {type.Namespace}  Type: {type.Name}");
    }

    [Fact]
    public void View_Interfaces()
    {
        foreach (TypeInfo type in _assembly.DefinedTypes
            .Where(t => t.IsInterface)
            .OrderBy(x => x.FullName))
            Write(type.FullName!);
    }

    [Fact]
    public void View_Abstract()
    {
        foreach (TypeInfo type in _assembly.DefinedTypes
            .Where(t => t is { IsAbstract: true, IsSealed: false, IsInterface: false })
            .OrderBy(x => x.FullName))
            Write(type.FullName!);
    }

    [Fact]
    public void View_Static_Types()
    {
        foreach (TypeInfo type in _assembly.DefinedTypes
            .Where(t => t is { IsAbstract: true, IsSealed: true })
            .OrderBy(x => x.FullName))
            Write(type.FullName!);
    }

    [Fact]
    public void View_NonSealed_Types()
    {
        foreach (TypeInfo type in _assembly.DefinedTypes
            .Where(t => t is { IsSealed: false, IsInterface: false, IsAbstract: false })
            .OrderBy(x => x.FullName))
            Write(type.FullName!);
    }

    [Fact]
    public void View_Exported_Types()
    {
        foreach (TypeInfo type in _assembly
            .ExportedTypes
            .OrderBy(x => x.FullName).Cast<TypeInfo>())
            Write(type.FullName!);
    }

    [Fact]
    public void View_Public_Methods()
    {
        List<string> objectMethodNames = typeof(object)
            .GetTypeInfo()
            .DeclaredMethods
            .Select(method => method.Name)
            .ToList();

        foreach (TypeInfo type in _assembly.ExportedTypes.OrderBy(type => type.Name).Cast<TypeInfo>())
        {
            type.GetTypeInfo().DeclaredMethods
                .Where(method => method.IsPublic)
                .Where(m =>
                    !(m.Name.StartsWith('<') || m.Name.StartsWith("get_") || m.Name.StartsWith("set_") ||
                      objectMethodNames.Contains(m.Name)))
                .Select(m => type.Name + "." + m.Name)
                .Distinct()
                .OrderBy(s => s)
                .ToList()
                .ForEach(x => Write(x));
        }
    }

    [Fact]
    public void List_Type_Attributes()
    {
        foreach (var group in _types
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
        foreach (var group in _types
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

}
