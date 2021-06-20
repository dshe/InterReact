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
    public partial class A
    {
        private readonly int X;
        public A()
        {
            X = Init();
        }
        public override string ToString() => "A";
    }
    public partial class A
    {
        public int Init()
        {
            return 1;
        }
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
