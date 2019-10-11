using InterReact.Messages;
using InterReact.Tests.Utility;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Xunit;
using Xunit.Abstractions;

namespace InterReact.Tests
{
    public class Class1
    {
        public int RequestId { get; } = 1;
    }

    public class Class2: BaseUnitTest
    {
        public Class2(ITestOutputHelper output) : base(output) { }

        [Fact]
        public void Test1()
        {
            //var contract = new Faker<Contract>().Generate();
            //Logger.LogDebug(JsonConvert.SerializeObject(contract, Formatting.None));
            var instance = new Class1();
            instance.SetProperty("RequestId", 123);
            Assert.Equal(123, instance.RequestId);
            ;
        }


    }
}
