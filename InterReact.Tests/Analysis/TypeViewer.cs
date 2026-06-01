using System.Reflection;
namespace Analysis;

public class Type_Viewer(ITestOutputHelper output) : OutputHelperTestBase(output)
{
    private static readonly Assembly _assembly = typeof(InterReactClient).Assembly;

    [Fact]
    public void View_Namespaces()
    {
        string[] spaces = _assembly.DefinedTypes.Select(t => t.Namespace ?? "").Distinct().OrderBy(x => x).ToArray();
        foreach (string? ns in spaces)
              Write(ns);
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

}
