using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using InterReact.Messages;
using InterReact.Enums;
using InterReact.StringEnums;
using Stringification;
using Xunit;
using Xunit.Abstractions;
using InterReact.Core;
using InterReact.Interfaces;

namespace InterReact.Tests.Utility.AutoData
{
    public class TestClass
    {
        public int First { get; set; }
        public IList<ContractComboLeg> ComboLegs { get; } = new List<ContractComboLeg>();
        public TestClass() { }
        public IReadOnlyList<Tag> SecurityIds { get; } = new List<Tag>(); // output
    }

    public class AutoDataTests : BaseUnitTest
    {
        public AutoDataTests(ITestOutputHelper output) : base(output) {}

        private void TestCreate<T>()
        {
            var value = AutoData.Create<T>();
            Logger.LogDebug(value.Stringify());
        }

        [Fact]
        public void Test_0()
        {
            TestCreate<string>();
            TestCreate<int>();
            TestCreate<long>();
            TestCreate<bool>();
            TestCreate<AlertType>();
            TestCreate<SecurityType>();
            TestCreate<ContractComboLeg>();
            TestCreate<Contract>();
            TestCreate<ContractData>();
            TestCreate<List<Contract>>();
            TestCreate<List<int>>();
            TestCreate<TestClass>();
        }

        [Fact]
        public void Test_1()
        {
            var c1 = AutoData.Create<ContractData>();
            var c2 = AutoData.Create<ContractData>();

            Logger.LogDebug(c1.Stringify());
            Logger.LogDebug("");
            Logger.LogDebug(c2.Stringify());

            Assert.NotEqual(c1.Stringify(), c2.Stringify());
        }
    }
}
