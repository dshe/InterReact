using InterReact;
using InterReact.SystemTests;
using System;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace InterReact.SystemTests.Contracts
{
    public class ContractsTest : TestCollectionBase
    {
        public ContractsTest(ITestOutputHelper output, TestFixture fixture) : base(output, fixture) { }
        [Fact]
        public async Task TestContractDataSingle()
        {
            var c = new Contract { SecurityType = SecurityType.Stock, Symbol = "IBM", Currency = "USD", Exchange = "SMART" };
            var item = await Client.Services
                .CreateContractDetailsObservable(c)
                .Timeout(TimeSpan.FromSeconds(10))
                .OfTypeUnionSource<ContractDetails>()
                .ToList();
            Assert.Equal(1, item.Count);
        }

        [Fact]
        public async Task TestContractDataMultiple()
        {
            var c = new Contract { SecurityType = SecurityType.Stock, Symbol = "IBM", Currency = "USD" };
            var list = await Client.Services.CreateContractDetailsObservable(c).ToList();
            Assert.NotEmpty(list); // multiple exchanges
        }

        [Fact]
        public async Task TestContractDataContractId()
        {
            var contract = new Contract { ContractId = 8314 };
            var cds = await Client.Services
                .CreateContractDetailsObservable(contract)
                .OfTypeUnionSource<ContractDetails>()
                .ToList();
            Assert.Equal("IBM", cds.Single().Contract.Symbol);
        }

        [Fact]
        public async Task TestContractDataSecurityId()
        {
            var c = new Contract { SecurityIdType = SecurityIdType.Isin, SecurityId = "IE00B5BMR087" };
            var list = await Client.Services.CreateContractDetailsObservable(c).ToList();
            Assert.True(list.Count > 1); // multiple exchanges
        }

        [Fact]
        public async Task TestContractDataRequestNotFound()
        {
            var contract = new Contract { ContractId = 99999 };
            var alert = await Assert.ThrowsAsync<InvalidOperationException>(async () => await Client.Services.CreateContractDetailsObservable(contract));
            //Assert.Equal(200, alert.Code);
        }

        [Fact]
        public async Task TestContractDataMultipleRequests()
        {
            var contract = new Contract { ContractId = 8314 };
            var task1 = Client.Services.CreateContractDetailsObservable(contract).ToList().ToTask();
            var task2 = Client.Services.CreateContractDetailsObservable(contract).ToList().ToTask();
            var task3 = Client.Services.CreateContractDetailsObservable(contract).ToList().ToTask();
            await Task.WhenAll(task1, task2, task3);
            Assert.Equal(1, task1.Result.Count);
            Assert.Equal(1, task2.Result.Count);
            Assert.Equal(1, task3.Result.Count);
        }

        [Fact]
        public async Task TestContractDataTimeout()
        {
            var contract = new Contract { SecurityType = SecurityType.Stock, Symbol = "IBM", Currency = "EUR" };
            await Assert.ThrowsAsync<TimeoutException>(async () => await Client.Services.CreateContractDetailsObservable(contract).Timeout(TimeSpan.Zero));
        }
    }
}
