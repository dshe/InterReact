using System.Collections.Generic;
using System.IO;
using System.Linq;
using InterReact.Extensions;
using InterReact.Messages;
using InterReact.Tests.Utility;
using Xunit;
using Xunit.Abstractions;

/*
namespace InterReact.Tests.UnitTests.Extensions
{
    public class ContractDataStrikeTest : BaseUnitTest
    {
        public ContractDataStrikeTest(ITestOutputHelper output) : base(output) {}

        [Fact]
        public void Test_Empty_Strike()
        {
            var input = new List<ContractData> { new ContractData() };

            Assert.Throws<InvalidDataException>(() => ContractDataStrikeEx.SelectStrike(input, 3, 0))
                .WriteMessageTo(Write);
        }

        [Fact]
        public void Test_Strike_Single()
        {
            var input = new List<ContractData> { new ContractData { Contract = { Strike = 100 } } };

            var output = ContractDataStrikeEx.SelectStrike(input, 0, 0);
            Assert.Equal(input.Single(), output.Single());

            output = ContractDataStrikeEx.SelectStrike(input, 0, 100);
            Assert.Equal(input.Single(), output.Single());

            output = ContractDataStrikeEx.SelectStrike(input, 1, 100);
            Assert.True(!output.Any());
        }

        [Fact]
        public void Test_Strike_Multiple()
        {
            var input = new List<ContractData>
            {
                new ContractData { Contract = { Strike = 100 } },
                new ContractData { Contract = { Strike = 110 } },
                new ContractData { Contract = { Strike = 100 } },
                new ContractData { Contract = { Strike = 80 } },
                new ContractData { Contract = { Strike = 220 } },
                new ContractData { Contract = { Strike = 50 } }
            };

            var output = ContractDataStrikeEx.SelectStrike(input, 0, 10);
            Assert.Equal(50, output.Single().Contract.Strike);

            output = ContractDataStrikeEx.SelectStrike(input, 1, 75);
            Assert.Equal(2, output.Count);

            output = ContractDataStrikeEx.SelectStrike(input, 0, 221);
            Assert.True(!output.Any());

            output = ContractDataStrikeEx.SelectStrike(input, 5, 0);
            Assert.True(!output.Any());
        }

    }
}
*/
