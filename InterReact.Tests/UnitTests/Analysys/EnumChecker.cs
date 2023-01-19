﻿using System.Reflection;

namespace Analysis;

public class Enum_Checker : UnitTestBase
{
    public Enum_Checker(ITestOutputHelper output) : base(output) { }

    private static readonly List<TypeInfo> EnumTypes =
        typeof(InterReactClient)
        .Assembly
        .DefinedTypes
        .Where(type => type.IsEnum)
        .OrderBy(x => x.Name)
        .ToList();

    [Fact]
    public void T01_Show_All()
    {
        foreach (var type in EnumTypes)
            Write($"{type.Name}: {string.Join(", ", type.GetEnumNames())}");
    }

    [Fact]
    public void T03_Show_Default_Values()
    {
        foreach (var type in EnumTypes)
        {
            var dict = new Dictionary<int, object>();
            foreach (var val in type.GetEnumValues())
            {
                if (val is null)
                    throw new Exception("invalid");
                var v = (int)val;
                if (dict.ContainsKey(v))
                    throw new Exception($"{type.Name} value {val} has duplicate value {v}.");
                dict.Add(v, val);
            }
            if (dict.ContainsKey(0))
                Write($"{type.Name}: {dict[0]}");
        }
    }

    [Fact]
    public void T04_Show_With_No_Default_Value()
    {
        foreach (var type in EnumTypes)
        {
            if (type.GetEnumValues().Cast<object>().Select(x => (int)x).All(x => x != 0))
                Write($"{type.Name} does not contain an enum of value 0");
        }
    }
}
