using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;

namespace Contracts;

public class ContractDetail : TestCollectionBase
{
    public ContractDetail(ITestOutputHelper output, TestFixture fixture) : base(output, fixture) { }

    private async Task<IList<IHasRequestId>> MakeContractDetailsRequest(Contract contract)
    {
        int requestId = Client.Request.GetNextId();

        var task = Client
            .Response
            .OfType<IHasRequestId>()
            .Where(x => x.RequestId == requestId)
            .TakeUntil(x => x is Alert || x is ContractDetailsEnd)
            .Where(x => x is not ContractDetailsEnd)
            .ToList()
            .ToTask(); // start task

        Client.Request.RequestContractDetails(requestId, contract);

        IList<IHasRequestId> messages = await task;

        return messages;
    }

    [Fact]
    public async Task SingleTest()
    {
        Contract contract = new()
        { 
            SecurityType = SecurityType.Stock, 
            Symbol = "IBM", 
            Currency = "USD", 
            Exchange = "SMART" 
        };

        IList<IHasRequestId> messages = await MakeContractDetailsRequest(contract);

        IHasRequestId message = messages.Single();
        Assert.IsType<ContractDetails>(message);
    }

    [Fact]
    public async Task MultiTest()
    {
        Contract contract = new()
        { 
            SecurityType = SecurityType.Stock, 
            Symbol = "IBM", 
            Currency = "USD" 
        };

        IList<IHasRequestId> messages = await MakeContractDetailsRequest(contract);

        Assert.True(messages.Count > 1);
        Assert.True(messages.All(x => x is ContractDetails));
    }

    [Fact]
    public async Task InvalidTest()
    {
        Contract contract = new()
        { 
            SecurityType = SecurityType.Stock, 
            Symbol = "ThisIsAnInvalidSymbol", 
            Currency = "SMART", 
            Exchange = "SMART" 
        };

        IList<IHasRequestId> messages = await MakeContractDetailsRequest(contract);

        Assert.Single(messages);
        Alert alert = Assert.IsType<Alert>(messages.Single());
        Write(alert.Message);
    }
}
