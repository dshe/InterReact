using System.Reactive.Linq;
using System.Threading.Tasks;
using InterReact;
using Xunit;
using InterReact.SystemTests;
using Xunit.Abstractions;

namespace SystemTests.FundamentalData
{
    public class FundamentalDataTests : BaseTest
    {
        public FundamentalDataTests(ITestOutputHelper output) : base(output) { }

        [Fact]
        public async Task TestFundamentalData()
        {
            var contract = new Contract { SecurityType = SecurityType.Stock, Symbol = "F", Currency = "USD", Exchange = "NYSE" };

            var observable = Client.Services.CreateFundamentalDataObservable(contract, FundamentalDataReportType.CompanyOverview);

            var xml = await observable;
        }
    }
}