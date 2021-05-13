using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xunit;
using Xunit.Abstractions;

namespace InterReact.UnitTests.Analysis
{
    public class Enum_Checker : BaseTest
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
                Write($"{type.Name}");
        }

        [Fact]
        public void T02_Show_All_With_Values()
        {
            foreach (var type in EnumTypes)
                Write($"{type.Name}: {string.Join(", ", type.GetEnumNames())}\n");
        }

        [Fact]
        public void T03_Show_Default_Values()
        {
            foreach (var type in EnumTypes)
            {
                var dict = new Dictionary<int, object>();
                foreach (var val in type.GetEnumValues())
                {
                    if (val == null)
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
}
