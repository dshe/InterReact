using System;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using InterReact.Messages;
using InterReact.StringEnums;
using Xunit;

namespace InterReact.Tests.SystemTests
{
    public partial class SystemTest
    {
        [Fact]
        public async Task TestContractDetailsSingle()
        {
            var c = new Contract { SecurityType = SecurityType.Stock, Symbol = "IBM", Currency = "USD", Exchange = "SMART"};
            var item = await Client.Services.ContractDetailsObservable(c).Timeout(TimeSpan.FromSeconds(10));
            Assert.Equal(1, item.Count);
        }

        [Fact]
        public async Task TestContractDetailsMultiple()
        {
            var c = new Contract { SecurityType = SecurityType.Stock, Symbol = "IBM", Currency = "USD" };
            var list = await Client.Services.ContractDetailsObservable(c);
            Assert.NotEmpty(list); // multiple exchanges
        }

        [Fact]
        public async Task TestContractDetailsContractId()
        {
            var contract = new Contract { ContractId = 8314 };
            var cds = await Client.Services.ContractDetailsObservable(contract);
            Assert.Equal("IBM", cds.Single().Contract.Symbol);
        }

        [Fact]
        public async Task TestContractDetailsSecurityId()
        {
            var c = new Contract {SecurityIdType = SecurityIdType.Isin, SecurityId = "IE00B5BMR087"};
            var list = await Client.Services.ContractDetailsObservable(c);
            Assert.True(list.Count > 1); // multiple exchanges
        }

        [Fact]
        public async Task TestContractDetailsRequestNotFound()
        {
            var contract = new Contract { ContractId = 99999 };
            var ex = await Assert.ThrowsAsync<AlertException>(async () => await Client.Services.ContractDetailsObservable(contract));
            Assert.Equal(200, ex.Alert.Code);
        }

        [Fact]
        public async Task TestContractDetailsMultipleRequests()
        {
            var contract = new Contract { ContractId = 8314 };
            var task1 = Client.Services.ContractDetailsObservable(contract).ToTask();
            var task2 = Client.Services.ContractDetailsObservable(contract).ToTask();
            var task3 = Client.Services.ContractDetailsObservable(contract).ToTask();
            await Task.WhenAll(task1, task2, task3);
            Assert.Equal(1, task1.Result.Count);
            Assert.Equal(1, task2.Result.Count);
            Assert.Equal(1, task3.Result.Count);
        }

        [Fact]
        public async Task TestContractDetailsTimeout()
        {
            var contract = new Contract { SecurityType = SecurityType.Stock, Symbol = "IBM", Currency = "EUR" };
            await Assert.ThrowsAsync<TimeoutException>(async () => await Client.Services.ContractDetailsObservable(contract).Timeout(TimeSpan.Zero));
        }
    }
}
