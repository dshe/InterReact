using System.Reactive.Linq;
using System.Threading.Tasks;
using InterReact.Enums;
using InterReact.Messages;
using InterReact.Tests.Utility;
using Xunit;
using Xunit.Abstractions;

namespace InterReact.Tests.SystemTests.Other
{
    public class FinancialAdvisorTests : BaseSystemTest
    {
        public FinancialAdvisorTests(SystemTestFixture fixture, ITestOutputHelper output) : base(fixture, output) { }

        [Fact]
        public async Task TestFinancialAdvisorConfiguration()
        {
            var observable = Client.Services.FinancialAdvisorConfigurationObservable(FinancialAdvisorDataType.Profiles);
            await Assert.ThrowsAsync<Alert>(async () => await observable);
            // Throws: Not a financial advisor account.
        }

    }
}
