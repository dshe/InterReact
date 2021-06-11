using InterReact;
using InterReact.SystemTests;
using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace InterReact.SystemTests.Service
{
    public class FinancialAdvisorTests : TestCollectionBase
    {
        public FinancialAdvisorTests(ITestOutputHelper output, TestFixture fixture) : base(output, fixture) { }

        [Fact]
        public async Task TestFinancialAdvisorConfiguration()
        {
            var observable = Client.Services.CreateFinancialAdvisorConfigurationObservable(FinancialAdvisorDataType.Profiles);
            await Assert.ThrowsAsync<InvalidOperationException>(async () => await observable);
            // Throws: Not a financial advisor account.
        }

    }
}
