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
    public class ContractDataExpiryTest : BaseUnitTest
    {
        public ContractDataExpiryTest(ITestOutputHelper output) : base(output) {}


        /*
        [Fact]
        public void Test_Empty_Expiry()
        {
            var xx = new Mock<ContractData>();
            xx.SetupGet<Contract>(x => new Contract());
            //xx.Object.

            //var input = new List<ContractData> { new ContractData() };
            var input = new List<ContractData> { xx.Object };
            Assert.Throws<InvalidDataException>(() => ContractDataExpiryEx.ContractDataExpiryFilter(input, 0))
                .WriteMessageTo(Write);
        }

        [Fact]
        public void Test_Expiry_Single()
        {
            var input = new List<ContractData> { new ContractData { Contract = { LastTradeDateOrContractMonth = "202012" } } };
           
            var output = ContractDataExpiryEx.ContractDataExpiryFilter(input, 0);
            Assert.Equal(input.Single(), output.Single());
        }

        [Fact]
        public void Test_Expiry_Multiple()
        {
            var input = new List<ContractData>
            {
                new ContractData { Contract = { LastTradeDateOrContractMonth = "202012" } },
                new ContractData { Contract = { LastTradeDateOrContractMonth = "202003" } },
                new ContractData { Contract = { LastTradeDateOrContractMonth = "202011" } },
                new ContractData { Contract = { LastTradeDateOrContractMonth = "202001" } },
                new ContractData { Contract = { LastTradeDateOrContractMonth = "202007" } },
                new ContractData { Contract = { LastTradeDateOrContractMonth = "202003" } }
            };

            var output = ContractDataExpiryEx.ContractDataExpiryFilter(input, 0);
            Assert.Equal(input[3], output.Single());

            output = ContractDataExpiryEx.ContractDataExpiryFilter(input, 1);
            Assert.Equal(2, output.Count);

            output = ContractDataExpiryEx.ContractDataExpiryFilter(input, 4);
            Assert.Equal(input[0], output.Single());

            output = ContractDataExpiryEx.ContractDataExpiryFilter(input, 5);
            Assert.True(!output.Any());
        }
        */
    }
}
