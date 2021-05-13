/*
namespace InterReact.UnitTests.Extensions
{
    public class ContractDataSmartTest : BaseUnitTest
    {
        public ContractDataSmartTest(ITestOutputHelper output) : base(output) { }

        [Fact]
        public void Test_Single()
        {
            var input = new List<ContractData> { new ContractData { Contract = { Exchange = "SMART" }}};
            var output = ContractDataSmartEx.ContractDataSmartFilter(input);
            Assert.Equal("SMART", output.Single().Contract.Exchange);

            input = new List<ContractData> { new ContractData { Contract = { Exchange = "other" } } };
            output = ContractDataSmartEx.ContractDataSmartFilter(input);
            Assert.Equal("other", output.Single().Contract.Exchange);
        }

        [Fact]
        public void Test_Multiple()
        {
            var input = new List<ContractData>
            {
                new ContractData { Contract = { Exchange = "other" } },
                new ContractData { Contract = { Exchange = "SMART" } }
            };
            var output = ContractDataSmartEx.ContractDataSmartFilter(input);
            Assert.Equal("SMART", output.Single().Contract.Exchange);
        }

        [Fact]
        public void Test_Multiple_ContractId()
        {
            var input = new List<ContractData>
            {
                new ContractData { Contract = { ContractId = 42, Exchange = "other" } },
                new ContractData { Contract = { ContractId = 43, Exchange = "SMART" } },
                new ContractData { Contract = { ContractId = 44, Exchange = "other" } },
                new ContractData { Contract = { ContractId = 44, Exchange = "SMART" } },
                new ContractData { Contract = { ContractId = 45, Exchange = "other" } },
                new ContractData { Contract = { ContractId = 45, Exchange = "other" } }
            };
            var output = ContractDataSmartEx.ContractDataSmartFilter(input);
            Assert.Equal(5, output.Count);
            Assert.Equal(2, output.Count(cd => cd.Contract.Exchange == "SMART"));
        }

    }
}
*/
