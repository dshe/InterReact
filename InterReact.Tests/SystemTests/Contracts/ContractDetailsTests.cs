using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace InterReact.SystemTests.Contracts;

public class ContractDetailsTests : TestCollectionBase
{
    public ContractDetailsTests(ITestOutputHelper output, TestFixture fixture) : base(output, fixture) { }

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

        Alert? alert = messages.OfType<Alert>().Where(alert => alert.IsFatal).FirstOrDefault();
        if (alert is not null)
            throw new AlertException(alert);

        return messages;
    }

    [Fact]
    public async Task TestSingle()
    {
        Contract contract = new()
        { SecurityType = SecurityType.Stock, Symbol = "IBM", Currency = "USD", Exchange = "SMART" };

        IList<IHasRequestId> messages = await MakeRequest(contract);

        IHasRequestId message = messages.Single();
        Assert.IsType<ContractDetails>(message);
    }

    [Fact]
    public async Task TestMulti()
    {
        Contract contract = new()
        { SecurityType = SecurityType.Stock, Symbol = "IBM", Currency = "USD" };

        IList<IHasRequestId> messages = await MakeRequest(contract);

        Assert.True(messages.Count > 1);
        Assert.True(messages.All(x => x is ContractDetails));
    }

    [Fact]
    public async Task TestInvalid()
    {
        Contract contract = new()
        { SecurityType = SecurityType.Stock, Symbol = "ThisIsAnInvalidSymbol", Currency = "SMART", Exchange = "SMART" };

        var alertException = await Assert.ThrowsAsync<AlertException>(async () => await MakeRequest(contract));

        Write(alertException.Message);
    }
}

