using System.Collections.Generic;
using System.IO;
using System.Linq;
using InterReact.Extensions;
using InterReact.Messages;
using InterReact.Tests.Utility;
using Xunit;
using Xunit.Abstractions;

namespace InterReact.Tests.UnitTests.Extensions
{
    public class ContractDetailsStrikeTest : BaseUnitTest
    {
        public ContractDetailsStrikeTest(ITestOutputHelper output) : base(output) {}

        [Fact]
        public void Test_Empty_Strike()
        {
            var input = new List<ContractDetails> { new ContractDetails() };

            Assert.Throws<InvalidDataException>(() => ContractDetailsStrikeEx.SelectStrike(input, 3, 0))
                .WriteMessageTo(Write);
        }

        [Fact]
        public void Test_Strike_Single()
        {
            var input = new List<ContractDetails> { new ContractDetails { Contract = { Strike = 100 } } };

            var output = ContractDetailsStrikeEx.SelectStrike(input, 0, 0);
            Assert.Equal(input.Single(), output.Single());

            output = ContractDetailsStrikeEx.SelectStrike(input, 0, 100);
            Assert.Equal(input.Single(), output.Single());

            output = ContractDetailsStrikeEx.SelectStrike(input, 1, 100);
            Assert.True(!output.Any());
        }

        [Fact]
        public void Test_Strike_Multiple()
        {
            var input = new List<ContractDetails>
            {
                new ContractDetails { Contract = { Strike = 100 } },
                new ContractDetails { Contract = { Strike = 110 } },
                new ContractDetails { Contract = { Strike = 100 } },
                new ContractDetails { Contract = { Strike = 80 } },
                new ContractDetails { Contract = { Strike = 220 } },
                new ContractDetails { Contract = { Strike = 50 } }
            };

            var output = ContractDetailsStrikeEx.SelectStrike(input, 0, 10);
            Assert.Equal(50, output.Single().Contract.Strike);

            output = ContractDetailsStrikeEx.SelectStrike(input, 1, 75);
            Assert.Equal(2, output.Count);

            output = ContractDetailsStrikeEx.SelectStrike(input, 0, 221);
            Assert.True(!output.Any());

            output = ContractDetailsStrikeEx.SelectStrike(input, 5, 0);
            Assert.True(!output.Any());
        }

    }
}
