using System.Reactive.Linq;
using System.Threading.Tasks;
using InterReact;
using InterReact.SystemTests;
using Xunit;
using Xunit.Abstractions;

namespace SystemTests.Other
{
    public class FinancialAdvisorTests : BaseTest
    {
        public FinancialAdvisorTests(ITestOutputHelper output) : base(output) { }

        [Fact]
        public async Task TestFinancialAdvisorConfiguration()
        {
            var observable = Client.Services.CreateFinancialAdvisorConfigurationObservable(FinancialAdvisorDataType.Profiles);
            await Assert.ThrowsAsync<Alert>(async () => await observable);
            // Throws: Not a financial advisor account.
        }

    }
}
