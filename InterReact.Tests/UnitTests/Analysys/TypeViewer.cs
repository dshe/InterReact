using System.Linq;
using System.Reflection;
using Xunit;
using Xunit.Abstractions;

namespace InterReact.UnitTests.Analysis;

public class Type_Viewer : UnitTestsBase
{
    public Type_Viewer(ITestOutputHelper output) : base(output) { }

    private static readonly Assembly Assembly = typeof(InterReactClient).Assembly;

    [Fact]
    public void View_Interfaces()
    {
        foreach (var type in Assembly.DefinedTypes.Where(t => t.IsInterface).OrderBy(x => x.FullName))
            Write(type.FullName!);
    }

    [Fact]
    public void View_Abstract()
    {
        foreach (var type in Assembly.DefinedTypes.Where(t => t.IsAbstract && !t.IsSealed && !t.IsInterface).OrderBy(x => x.FullName))
            Write(type.FullName!);
    }

    [Fact]
    public void View_Static_Types()
    {
        foreach (var type in Assembly.DefinedTypes.Where(t => t.IsAbstract && t.IsSealed).OrderBy(x => x.FullName))
            Write(type.FullName!);
    }

    [Fact]
    public void View_NonSealed_Types()
    {
        foreach (var type in Assembly.DefinedTypes.Where(t => !t.IsSealed && !t.IsInterface && !t.IsAbstract).OrderBy(x => x.FullName))
            Write(type.FullName!);
    }

    [Fact]
    public void View_Exported_Types()
    {
        foreach (var type in Assembly.ExportedTypes.OrderBy(x => x.FullName))
            Write(type.FullName!);
    }

    [Fact]
    public void View_Public_Methods()
    {
        var objectMethodNames = typeof(object).GetTypeInfo().DeclaredMethods.Select(method => method.Name).ToList();

        foreach (var type in Assembly.ExportedTypes.OrderBy(type => type.Name))
        {
            type.GetTypeInfo().DeclaredMethods
                //.Where(method => method.IsPublic)
                .Where(m =>
                    !(m.Name.StartsWith("<") || m.Name.StartsWith("get_") || m.Name.StartsWith("set_") ||
                      objectMethodNames.Contains(m.Name)))
                .Select(m => type.Name + "." + m.Name)
                .Distinct()
                .OrderBy(s => s)
                .ToList()
                .ForEach(x => Write(x));
        }
    }

}
