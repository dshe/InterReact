using InterReact;
using InterReact.SystemTests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace InterReact.SystemTests.Contracts
{
    public class ContractDetailsServiceTests : TestCollectionBase
    {
        public ContractDetailsServiceTests(ITestOutputHelper output, TestFixture fixture) : base(output, fixture) { }
        [Fact]
        public async Task TestContractDetailsSingle()
        {
            Contract contract = new() { SecurityType = SecurityType.Stock, Symbol = "IBM", Currency = "USD", Exchange = "SMART" };

            IList<ContractDetails> cds = await Client
                .Services
                .CreateContractDetailsObservable(contract)
                .ToList();

            Assert.Single(cds);
        }

        [Fact]
        public async Task TestContractDetailsMultiple()
        {
            Contract contract = new() { SecurityType = SecurityType.Stock, Symbol = "IBM", Currency = "USD" };

            var cds = await Client
                .Services
                .CreateContractDetailsObservable(contract)
                .ToList();
            
            Assert.True(cds.Count > 1); // multiple exchanges
        }

        [Fact]
        public async Task TestContractDetailsRequestNotFound()
        {
            var contract = new Contract { ContractId = 99999 };

            var alertException = await Assert.ThrowsAsync<AlertException>(async () => await Client
                .Services
                .CreateContractDetailsObservable(contract));

            Assert.Equal(200, alertException.Alert.Code);
        }

        [Fact]
        public async Task TestContractDetailsTimeout()
        {
            var contract = new Contract { SecurityType = SecurityType.Stock, Symbol = "IBM", Currency = "EUR" };

            await Assert.ThrowsAsync<TimeoutException>(async () => await Client
                .Services
                .CreateContractDetailsObservable(contract)
                .Timeout(TimeSpan.Zero));
        }
    }
}
