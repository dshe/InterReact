using InterReact;
using InterReact.SystemTests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace InterReact.SystemTests.Contracts
{
    public class ContractDetailsRequestTests : TestCollectionBase
    {
        public ContractDetailsRequestTests(ITestOutputHelper output, TestFixture fixture) : base(output, fixture) { }

        private async Task<IList<IHasRequestId>> MakeRequest(Contract contract)
        {
            int id = Client.Request.GetNextId();

            var task = Client
                .Response
                .OfType<IHasRequestId>()
                .Where(x => x.RequestId == id)
                .TakeUntil(x => x is Alert || x is ContractDetailsEnd)
                .Where(x => x is not ContractDetailsEnd)
                .ToList()
                .ToTask(); // start task

            Client.Request.RequestContractDetails(id, contract);

            IList<IHasRequestId> messages = await task;

            return messages;
        }

        [Fact]
        public async Task TestRequestSingleContractDetails()
        {
            Contract contract = new()
                { SecurityType = SecurityType.Stock, Symbol = "IBM", Currency = "USD", Exchange = "SMART" };

            IList<IHasRequestId> messages = await MakeRequest(contract);

            IHasRequestId message = messages.Single();
            Assert.IsType<ContractDetails>(message);
        }

        [Fact]
        public async Task TestRequestMultiContractDetails()
        {
            Contract contract = new()
                { SecurityType = SecurityType.Stock, Symbol = "IBM", Currency = "USD" };

            IList<IHasRequestId> messages = await MakeRequest(contract);

            Assert.True(messages.Count > 1);
            Assert.True(messages.All(x => x is ContractDetails));
        }

        [Fact]
        public async Task TestRequestBadContractDetails()
        {
            Contract contract = new()
                { SecurityType = SecurityType.Stock, Symbol = "ThisIsAnInvalidSymbol", Currency = "SMART", Exchange = "SMART" };

            IList<IHasRequestId> messages = await MakeRequest(contract);

            IHasRequestId message = messages.Single();
            Alert alert = (Alert)message;
            Write(alert.Message);
        }
    }
}

