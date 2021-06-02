using InterReact;
using InterReact.SystemTests;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace InterReact.SystemTests.Service
{
    public class FundamentalDataTests : TestCollectionBase
    {
        public FundamentalDataTests(ITestOutputHelper output, TestFixture fixture) : base(output, fixture) { }

        [Fact]
        public async Task TestFundamentalData()
        {
            var contract = new Contract { SecurityType = SecurityType.Stock, Symbol = "F", Currency = "USD", Exchange = "NYSE" };

            var observable = Client.Services.CreateFundamentalDataObservable(contract, FundamentalDataReportType.CompanyOverview);

            var xml = await observable;
        }
    }
}