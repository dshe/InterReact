using System.Reactive.Linq;
using System.Threading.Tasks;
using InterReact.Enums;
using InterReact.Messages;
using InterReact.StringEnums;
using Xunit;
using InterReact.Tests.Utility;
using Xunit.Abstractions;

namespace InterReact.Tests.SystemTests.FundamentalData
{
    public class FundamentalDataTests : BaseSystemTest
    {
        public FundamentalDataTests(SystemTestFixture fixture, ITestOutputHelper output) : base(fixture, output) {}

        [Fact]
        public async Task TestFundamentalData()
        {
            var contract = new Contract {SecurityType = SecurityType.Stock, Symbol = "F", Currency = "USD", Exchange = "NYSE"};

            var observable = Client.Services.FundamentalDataObservable(contract, FundamentalDataReportType.CompanyOverview);

            await Assert.ThrowsAsync<Alert>(async () => await observable);
        }
    }
}