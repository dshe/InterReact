using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
using InterReact;
using Stringification;
using InterReact.Tests.Utility;
using Microsoft.Extensions.Logging;
using Xunit;
using Xunit.Abstractions;
using System;

namespace InterReact.Tests.UnitTests.Analysis
{
    public class AttributeViewer : BaseUnitTest
    {
        private static readonly IEnumerable<TypeInfo> types =
            typeof(InterReactClient)
            .GetTypeInfo()
            .Assembly
            .DefinedTypes
            .Where(t => !t.Name.Contains("<>"));

        public AttributeViewer(ITestOutputHelper output) : base(output) {}

        [Fact]
        public void Find_Type_Attributes()
        {
            foreach (var group in types
                .Select(ti => ti.GetCustomAttributes(true).Select(a => new { a, ti }))
                .SelectMany(x => x)
                .Where(x =>
                    !(x.a is CompilerGeneratedAttribute) &&
                    !(x.a is ExtensionAttribute))
                .GroupBy(x => x.a))
            {
                Logger.LogDebug(group.Key.ToString());
                foreach (var a in group.OrderBy(x => x.ti?.FullName))
                    Logger.LogDebug($"    {a.ti?.FullName}");
            }
        }

        [Fact]
        public void Find_Member_Attributes()
        {
            foreach (var group in types
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
                Logger.LogDebug(group.Key.ToString());
                foreach (var a in group.OrderBy(x=> x.type?.FullName + x.method?.Name))
                    Logger.LogDebug($"     {a.type?.FullName}  {a.method?.Name}");
            }
        }

        [Fact]
        public void AutoTypeAndStringify()
        {
            foreach (var type in types.Where(t =>
                t.IsClass && t.IsSealed &&
                t.Namespace != null && t.Namespace.Contains(".Messages")).ToList())
            {
                try
                {
                    var instance = AutoData.Create(type);
                    Assert.NotNull(instance);
                    var str = instance!.Stringify();
                    Logger.LogDebug(str + "\n");
                }
                catch (Exception e)
                {
                    Logger.LogError(e, $"Type: {type.Name}");
                    throw;
                }
            }
        }

    }
}
