using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using InterReact;
using InterReact.Extensions;
using InterReact.SystemTests;
using Xunit;
using Xunit.Abstractions;
using System.Threading;
using System;

namespace SystemTests.Contracts
{
    public class ContractTests : BaseTest
    {
        public ContractTests(ITestOutputHelper output) : base(output) { }

        [Fact]
        public async Task TestRequestContractData()
        {
            var cts = new CancellationTokenSource();

            var task = Client
                .Response
                .OfType<ContractDataEnd>()
                .FirstOrDefaultAsync().Timeout(TimeSpan.FromSeconds(10)).ToTask(cts.Token);

            Client.Request.RequestContractData(Id, Stock1);

            await task;
        }
    }
}

