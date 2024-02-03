using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;

namespace Contracts;

public class ContractDetail : CollectionTestBase
{
    public ContractDetail(ITestOutputHelper output, TestFixture fixture) : base(output, fixture) { }

    private async Task<IList<ContractDetails>> MakeContractDetailsRequest(Contract contract)
    {
        int id = Client.Request.GetNextId();

        Task<IList<ContractDetails>> task = Client
            .Response
            .WithRequestId(id)
            .AlertMessageToError()
            .TakeWhile(m => m is not ContractDetailsEnd)
            .Cast<ContractDetails>()
            .ToList()
            .ToTask();

        Client.Request.RequestContractDetails(id, contract);

        return await task;
    }

    [Fact]
    public async Task SingleTest()
    {
        Contract contract = new()
        {
            SecurityType = ContractSecurityType.Stock,
            Symbol = "IBM",
            Currency = "USD",
            Exchange = "SMART"
        };

        IList<ContractDetails> cds = await MakeContractDetailsRequest(contract);
        Assert.Single(cds);
    }

    [Fact]
    public async Task MultiTest()
    {
        Contract contract = new()
        {
            SecurityType = ContractSecurityType.Stock,
            Symbol = "IBM",
            Currency = "USD"
        };

        IList<ContractDetails> cds = await MakeContractDetailsRequest(contract);
        Assert.True(cds.Count > 1);
    }

    [Fact]
    public async Task InvalidTest()
    {
        Contract contract = new()
        {
            SecurityType = ContractSecurityType.Stock,
            Symbol = "ThisIsAnInvalidSymbol",
            Currency = "SMART",
            Exchange = "SMART"
        };

        await Assert.ThrowsAsync<AlertException>(() => MakeContractDetailsRequest(contract));
    }
}
