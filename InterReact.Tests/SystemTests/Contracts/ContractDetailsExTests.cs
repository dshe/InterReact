using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using InterReact.Extensions;
using InterReact.Messages;
using InterReact.StringEnums;
using Microsoft.Extensions.Logging;
using Xunit;

namespace InterReact.Tests.SystemTests
{
    public partial class SystemTest
    {
        [Fact]
        public async Task TestContractDataGetNextFuture()
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
                .ContractDataObservable(contract)
                .ContractDataExpirySelect();
            Logger.LogDebug(cd.Single().Contract.Stringify());
        }

        [Fact]
        public async Task TestContractDataOptionFind()
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
            var cds = await Client.Services.ContractDataObservable(contract);
            foreach (var cd in cds.OrderBy(x => x.Contract.LastTradeDateOrContractMonth))
                Logger.LogDebug(cd.Contract.LastTradeDateOrContractMonth);
            Assert.True(cds.All(cd => cd.Contract.Strike == 200));
        }
    }
}