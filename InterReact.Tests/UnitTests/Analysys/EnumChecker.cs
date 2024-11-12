using System.Reflection;
namespace Analysis;

public class Enum_Checker(ITestOutputHelper output) : UnitTestBase(output)
{
    private static readonly List<TypeInfo> EnumTypes = [.. typeof(InterReactClient)
        .Assembly
        .DefinedTypes
        .Where(type => type.IsEnum)
        .OrderBy(x => x.Name)];

    [Fact]
    public void T01_Show_All()
    {
        foreach (TypeInfo type in EnumTypes)
            Write($"{type.Name}: {string.Join(", ", type.GetEnumNames())}");
    }

    [Fact]
    public void T03_Show_Default_Values()
    {
        foreach (TypeInfo type in EnumTypes)
        {
            Dictionary<int, object> dict = [];
            foreach (object? val in type.GetEnumValues())
            {
                if (val is null)
                    throw new Exception("invalid");
                int v = (int)val;
                if (dict.ContainsKey(v))
                    throw new Exception($"{type.Name} value {val} has duplicate value {v}.");
                dict.Add(v, val);
            }
            if (dict.TryGetValue(0, out object? value))
                Write($"{type.Name}: {value}");
        }
    }

    [Fact]
    public void T04_Show_With_No_Default_Value()
    {
        foreach (TypeInfo type in EnumTypes)
        {
            if (type.GetEnumValues().Cast<object>().Select(x => (int)x).All(x => x != 0))
                Write($"{type.Name} does not contain an enum of value 0");
        }
    }
}
