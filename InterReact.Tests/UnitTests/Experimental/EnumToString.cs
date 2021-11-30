using System;
using Xunit.Abstractions;

namespace InterReact.UnitTests.Experimental;

public class EnumToString : UnitTestsBase
{
    public EnumToString(ITestOutputHelper output) : base(output) { }

    enum MyEnum { one, two, three };

    private void WriteEnumStrings(Type type)
    {
        Array values = Enum.GetValues(type);
        string typeName = type.Name;

        Write("{");
        foreach (var value in values)
        {
            Write($"\t{typeName}.{value} => nameof({typeName}.{value}),");
        }
        Write($"\t_ => throw new InvalidCastException(nameof({typeName}))");
        Write("};");
    }

    //[Fact]
    public void Test()
    {
        //WriteEnumStrings(typeof(MyEnum));
        WriteEnumStrings(typeof(TickType));
    }
}
