using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Stringification;
using InterReact.Extensions;
using Xunit;
using Xunit.Abstractions;
using InterReact.SystemTests;
using InterReact;

namespace SystemTests.Contracts
{
    public class ContractDataGetNextFuture : BaseTest
    {
        public ContractDataGetNextFuture(ITestOutputHelper output) : base(output) { }

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
            var cds = await Client.Services.CreateContractDataObservable(contract);
            var cd = cds.ContractDataExpiryFilter(0).Single();

            Write(cd.Stringify());
            Assert.Equal(.25, cd.MinimumTick);
        }
    }
    public class ContractDataOptionFind : BaseTest
    {
        public ContractDataOptionFind(ITestOutputHelper output) : base(output) { }

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
            var cds = await Client.Services.CreateContractDataObservable(contract);
            foreach (var cd in cds.OrderBy(x => x.Contract.LastTradeDateOrContractMonth))
                Write(cd.Contract.LastTradeDateOrContractMonth);
            Assert.True(cds.All(cd => cd.Contract.Strike == 200));
        }
    }
}