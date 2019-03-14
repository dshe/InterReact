using System.Collections.Generic;
using System.IO;
using System.Linq;
using InterReact.Extensions;
using InterReact.Messages;
using InterReact.Tests.Utility;
using Moq;
using Xunit;
using Xunit.Abstractions;

namespace InterReact.Tests.UnitTests.Extensions
{
    public class ContractDetailsExpiryTest : BaseUnitTest
    {
        public ContractDetailsExpiryTest(ITestOutputHelper output) : base(output) {}


        /*
        [Fact]
        public void Test_Empty_Expiry()
        {
            var xx = new Mock<ContractDetails>();
            xx.SetupGet<Contract>(x => new Contract());
            //xx.Object.

            //var input = new List<ContractDetails> { new ContractDetails() };
            var input = new List<ContractDetails> { xx.Object };
            Assert.Throws<InvalidDataException>(() => ContractDetailsExpiryEx.ContractDetailsExpiryFilter(input, 0))
                .WriteMessageTo(Write);
        }

        [Fact]
        public void Test_Expiry_Single()
        {
            var input = new List<ContractDetails> { new ContractDetails { Contract = { LastTradeDateOrContractMonth = "202012" } } };
           
            var output = ContractDetailsExpiryEx.ContractDetailsExpiryFilter(input, 0);
            Assert.Equal(input.Single(), output.Single());
        }

        [Fact]
        public void Test_Expiry_Multiple()
        {
            var input = new List<ContractDetails>
            {
                new ContractDetails { Contract = { LastTradeDateOrContractMonth = "202012" } },
                new ContractDetails { Contract = { LastTradeDateOrContractMonth = "202003" } },
                new ContractDetails { Contract = { LastTradeDateOrContractMonth = "202011" } },
                new ContractDetails { Contract = { LastTradeDateOrContractMonth = "202001" } },
                new ContractDetails { Contract = { LastTradeDateOrContractMonth = "202007" } },
                new ContractDetails { Contract = { LastTradeDateOrContractMonth = "202003" } }
            };

            var output = ContractDetailsExpiryEx.ContractDetailsExpiryFilter(input, 0);
            Assert.Equal(input[3], output.Single());

            output = ContractDetailsExpiryEx.ContractDetailsExpiryFilter(input, 1);
            Assert.Equal(2, output.Count);

            output = ContractDetailsExpiryEx.ContractDetailsExpiryFilter(input, 4);
            Assert.Equal(input[0], output.Single());

            output = ContractDetailsExpiryEx.ContractDetailsExpiryFilter(input, 5);
            Assert.True(!output.Any());
        }
        */
    }
}
