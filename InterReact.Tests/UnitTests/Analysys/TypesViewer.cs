using Stringification;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
using Xunit;
using Xunit.Abstractions;


namespace InterReact.UnitTests.Analysis
{
    public class Types_Viewer : BaseUnitTest
    {
        public Types_Viewer(ITestOutputHelper output) : base(output) { }

        private static readonly List<TypeInfo> Types =
            typeof(InterReactClient)
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
                .Where(x =>
                    !(x.a is CompilerGeneratedAttribute) &&
                    !(x.a is ExtensionAttribute))
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
                .Select(t => t.DeclaredMembers.Where(m => !m.Name.StartsWith("<")).OfType<MemberInfo>().Select(m => (t, m)))
                .SelectMany(x => x)
                //.Where(x => x.m != null) //.OfType<(TypeInfo, MemberInfo)>()

                .Select(x => x.m.GetCustomAttributes(false).Select(q => new { type = x.t, method = x.m, attr = q }))
                .SelectMany(x => x)
                .Where(x => !(
                        x.attr is CompilerGeneratedAttribute ||
                        x.attr is DebuggerHiddenAttribute ||
                        x.attr is SecuritySafeCriticalAttribute ||
                        x.attr is AsyncStateMachineAttribute ||
                        x.attr is DebuggerStepThroughAttribute))
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
            var type = typeof(ContractDetails);
            var value = AutoData.Create(type);
            Assert.NotNull(value);
            Write(value!.Stringify());
        }

        [Fact]
        public void Auto_Type_And_Stringify_All() // sometimes fails?
        {
            foreach (var type in Types.Where(t =>
                t.IsClass && t.IsSealed &&
                t.Namespace != null && t.Namespace.Contains(".Messages")).ToList())
            {
                try
                {
                    var instance = AutoData.Create(type);
                    Assert.NotNull(instance);
                    var str = instance!.Stringify();
                    Write(str + "\n");
                }
                catch (Exception e)
                {
                    Write($"Type: {type.Name} EXCEPTION\r\n{e}");
                    throw;
                }
            }
        }
    }
}
