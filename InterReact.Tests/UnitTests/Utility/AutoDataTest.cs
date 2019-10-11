using System.Collections.Generic;
using InterReact.Messages;
using InterReact.Enums;
using InterReact.StringEnums;
using Stringification;
using Xunit;
using Xunit.Abstractions;
using InterReact.Core;
using InterReact.Interfaces;
using InterReact.Tests.Utility;
using Microsoft.Extensions.Logging;
using System;

#nullable enable

namespace InterReact.Tests.UnitTests
{
    public class AutoDataTests : BaseUnitTest
    {
        public AutoDataTests(ITestOutputHelper output) : base(output) { }

        private void TestCreate<T>()
        {
            var value = AutoData.Create<T>();
            if (value == null)
                throw new Exception("null");
            Logger.LogDebug(value.Stringify());
        }

        [Fact]
        public void Test_0()
        {
            TestCreate<Tag>();
            TestCreate<AlertType>();
            TestCreate<SecurityType>();
            TestCreate<ContractComboLeg>();
            TestCreate<Contract>();
            TestCreate<List<Contract>>();
            TestCreate<ContractData>();
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

        [Fact]
        public void Test_2()
        {
            //Assert.NotEqual(c1.Stringify(), c2.Stringify());
        }


    }
}
