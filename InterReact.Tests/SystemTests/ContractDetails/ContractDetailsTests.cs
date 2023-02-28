using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;

namespace Contracts;

public class ContractDetail : TestCollectionBase
{
    public ContractDetail(ITestOutputHelper output, TestFixture fixture) : base(output, fixture) { }

    private async Task<IList<object>> MakeContractDetailsRequest(Contract contract)
    {
        int id = Client.Request.GetNextId();

        Task<IList<object>> task = Client
            .Response
            .WithRequestId(id)
            .TakeUntil(x => x is AlertMessage or ContractDetailsEnd)
            .Where(x => x is not ContractDetailsEnd)
            .ToList()
            .ToTask(); // start task

        Client.Request.RequestContractDetails(id, contract);

        IList<object> messages = await task;

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

        IList<object> messages = await MakeContractDetailsRequest(contract);

        object message = messages.Single();
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

        IList<object> messages = await MakeContractDetailsRequest(contract);

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

        IList<object> messages = await MakeContractDetailsRequest(contract);

        Assert.Single(messages);
        AlertMessage alert = Assert.IsType<AlertMessage>(messages.Single());
        Write(alert.Message);
    }
}
