using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using InterReact.Tests.Utility;
using Xunit;
using Xunit.Abstractions;

namespace InterReact.Tests.UnitTests.Analysis
{
    public class PrimitiveEnumChecks : BaseUnitTest
    {
        private static readonly IEnumerable<TypeInfo> EnumTypes =
            typeof(InterReactClient).GetTypeInfo().Assembly
            .DefinedTypes.Where(type => type.IsEnum).OrderBy(x => x.Name);

        public PrimitiveEnumChecks(ITestOutputHelper output) : base(output) {}

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
                if (type.GetEnumValues().Cast<object>().Select(x => (int) x).All(x => x != 0))
                    Write($"{type.Name} does not contain an enum of value 0");
            }
        }
    }
}
