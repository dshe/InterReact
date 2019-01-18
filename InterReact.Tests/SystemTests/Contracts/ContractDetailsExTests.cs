using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using InterReact.Extensions;
using InterReact.Messages;
using InterReact.StringEnums;
using Xunit;

namespace InterReact.Tests.SystemTests
{
    public partial class SystemTest
    {
        [Fact]
        public async Task TestContractDetailsGetNextFuture()
        {
            var contract = new Contract
            {
                SecurityType = SecurityType.Future,
                Symbol = "ES",
                Currency = "USD",
                Exchange = "GLOBEX"
            };
            var cd = await Client
                .Services
                .ContractDetailsObservable(contract)
                .ContractDetailsExpirySelect();
            Write(cd.Single().Contract.Stringify());
        }

        [Fact]
        public async Task TestContractDetailsOptionFind()
        {
            var contract = new Contract
            {
                SecurityType = SecurityType.Option,
                Right = RightType.Call,
                Symbol = "SPY",
                Currency = "USD",
                Exchange = "SMART",
                Multiplier = "100",
                Strike = 200
            };
            var cds = await Client.Services.ContractDetailsObservable(contract);
            foreach (var cd in cds.OrderBy(x => x.Contract.LastTradeDateOrContractMonth))
                Write(cd.Contract.LastTradeDateOrContractMonth);
            Assert.True(cds.All(cd => cd.Contract.Strike == 200));
        }
    }
}