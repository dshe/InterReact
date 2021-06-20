using InterReact;
using InterReact.SystemTests;
using Stringification;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace InterReact.SystemTests.Contracts
{
    public class ContractDataGetNextFuture : TestCollectionBase
    {
        public ContractDataGetNextFuture(ITestOutputHelper output, TestFixture fixture) : base(output, fixture) { }

        [Fact]
        public async Task Test()
        {
            var contract = new Contract
            {
                SecurityType = SecurityType.Future,
                Symbol = "ES",
                Currency = "USD",
                Exchange = "GLOBEX"
            };
            var cds = await Client.Services.CreateContractDetailsObservable(contract)
                .OfTypeUnionSource<ContractDetails>()
                .ToList();
            var cd = cds.ContractDataExpiryFilter(0).Single();

            Write(cd.Stringify());
            Assert.Equal(.25, cd.MinimumTick);
        }
    }
    public class ContractDataOptionFind : TestCollectionBase
    {
        public ContractDataOptionFind(ITestOutputHelper output, TestFixture fixture) : base(output, fixture) { }

        [Fact]
        public async Task Test()
        {
            var contract = new Contract
            {
                SecurityType = SecurityType.Option,
                Right = OptionRightType.Call,
                Symbol = "SPY",
                Currency = "USD",
                Exchange = "SMART",
                Multiplier = "100",
                Strike = 200
            };
            var cds = await Client.Services
                .CreateContractDetailsObservable(contract)
                .OfTypeUnionSource<ContractDetails>()
                .ToList();
            foreach (var cd in cds.OrderBy(x => x.Contract.LastTradeDateOrContractMonth))
                Write(cd.Contract.LastTradeDateOrContractMonth);
            Assert.True(cds.All(cd => cd.Contract.Strike == 200));
        }
    }
}