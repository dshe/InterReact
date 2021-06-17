using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace InterReact.UnitTests.Experimental
{
    public class A
    {
        public override string ToString() => "A";
    }
    public class B
    {
        public override string ToString() => "B";
    }
    public class C
    {
        public override string ToString() => "C";
    }


    public class TestClass : BaseUnitTest
    {
        public TestClass(ITestOutputHelper output) : base(output) { }

        [Fact]
        public void Test1()
        {
            var a = new A();
            var b = new B();
            var c = new C();

            List<object> list = new();
            list.Add(a);
            list.Add(b);

            list.Cast<C>().ToList().ForEach(x => Write(x.ToString()));

        }

    }
}
