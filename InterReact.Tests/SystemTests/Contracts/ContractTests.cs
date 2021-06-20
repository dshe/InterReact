using InterReact;
using InterReact.SystemTests;
using System;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace InterReact.SystemTests.Contracts
{
    public class ContractTests : TestCollectionBase
    {
        public ContractTests(ITestOutputHelper output, TestFixture fixture) : base(output, fixture) { }

        [Fact]
        public async Task TestRequestContractData()
        {
            var cts = new CancellationTokenSource();

            var task = Client
                .Response
                .OfType<ContractDetailsEnd>()
                .FirstOrDefaultAsync().Timeout(TimeSpan.FromSeconds(10)).ToTask(cts.Token);

            Client.Request.RequestContractDetails(Id, Stock1);

            await task;
        }
    }
}

