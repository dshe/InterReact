using System.Collections.Generic;
using System.Linq;
using InterReact.Extensions;
using InterReact.Messages;
using InterReact.Tests.Utility;
using Xunit;
using Xunit.Abstractions;
/*
namespace InterReact.Tests.UnitTests.Extensions
{
    public class ContractDetailsSmartTest : BaseUnitTest
    {
        public ContractDetailsSmartTest(ITestOutputHelper output) : base(output) { }

        [Fact]
        public void Test_Single()
        {
            var input = new List<ContractDetails> { new ContractDetails { Contract = { Exchange = "SMART" }}};
            var output = ContractDetailsSmartEx.ContractDetailsSmartFilter(input);
            Assert.Equal("SMART", output.Single().Contract.Exchange);

            input = new List<ContractDetails> { new ContractDetails { Contract = { Exchange = "other" } } };
            output = ContractDetailsSmartEx.ContractDetailsSmartFilter(input);
            Assert.Equal("other", output.Single().Contract.Exchange);
        }

        [Fact]
        public void Test_Multiple()
        {
            var input = new List<ContractDetails>
            {
                new ContractDetails { Contract = { Exchange = "other" } },
                new ContractDetails { Contract = { Exchange = "SMART" } }
            };
            var output = ContractDetailsSmartEx.ContractDetailsSmartFilter(input);
            Assert.Equal("SMART", output.Single().Contract.Exchange);
        }

        [Fact]
        public void Test_Multiple_ContractId()
        {
            var input = new List<ContractDetails>
            {
                new ContractDetails { Contract = { ContractId = 42, Exchange = "other" } },
                new ContractDetails { Contract = { ContractId = 43, Exchange = "SMART" } },
                new ContractDetails { Contract = { ContractId = 44, Exchange = "other" } },
                new ContractDetails { Contract = { ContractId = 44, Exchange = "SMART" } },
                new ContractDetails { Contract = { ContractId = 45, Exchange = "other" } },
                new ContractDetails { Contract = { ContractId = 45, Exchange = "other" } }
            };
            var output = ContractDetailsSmartEx.ContractDetailsSmartFilter(input);
            Assert.Equal(5, output.Count);
            Assert.Equal(2, output.Count(cd => cd.Contract.Exchange == "SMART"));
        }

    }
}
*/
