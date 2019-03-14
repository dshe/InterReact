using System.Collections.Generic;
using InterReact.Messages;
using InterReact.Enums;
using InterReact.StringEnums;
using Stringification;
using Xunit;
using Xunit.Abstractions;

namespace InterReact.Tests.Utility.AutoData
{
    public class TestClass
    {
        public int First { get; set; }
        public IList<ContractComboLeg> ComboLegs { get; } = new List<ContractComboLeg>();
    }

    public class AutoDataTests : BaseUnitTest
    {
        public AutoDataTests(ITestOutputHelper output) : base(output) {}

        private void TestCreate<T>()
        {
            var value = AutoData.Create<T>();
            Write(value.Stringify());
        }

        [Fact]
        public void Test_0()
        {
            //TestCreate<string>();
            //TestCreate<int>();
            //TestCreate<long>();
            //TestCreate<bool>();
            //TestCreate<SecurityType>();
            TestCreate<TestClass>();
            //TestCreate<ContractComboLeg>();
            //TestCreate<Contract>();
            //TestCreate<ContractDetails>();
            //TestCreate<List<Contract>>();
            //TestCreate<List<int>>();
        }

        [Fact]
        public void Test_1()
        {
            var c1 = AutoData.Create<ContractDetails>();
            var c2 = AutoData.Create<ContractDetails>();

            Write(c1.Stringify());
            Write("");
            Write(c2.Stringify());

            Assert.NotEqual(c1.Stringify(), c2.Stringify());
        }
    }
}
