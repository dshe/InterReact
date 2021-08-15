using Stringification;
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
            Union<FundamentalData, Alert> union = await Client
                .Services
                .CreateFundamentalDataObservable(StockContract1, FundamentalDataReportType.CompanyOverview);

            object source = union.Source;

            if (source is Alert alert)
                Write(alert.Stringify());
            else
                Write(source.Stringify());

         }
    }
}