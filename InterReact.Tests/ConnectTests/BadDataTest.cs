using Microsoft.Extensions.Logging;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
namespace ConnectTests;

public class BadData(ITestOutputHelper output) : OutputHelperTestBase(output, LogLevel.Debug)
{
    [Fact]
    public async Task BadRequestTestAsync()
    {
        IInterReactClient client = await InterReactClient.CreateAsync(options =>
            options.LogFactory = LogFactory, TestContext.Current.CancellationToken);

        int id = client.Request.GetNextId();

        Task<IHasRequestId> task = client
            .Response
            .WithRequestId(id)
            .FirstAsync()
            .Timeout(TimeSpan.FromSeconds(6))
            .ToTask();

        Contract contract = new() {
            SecurityType = ContractSecurityType.Stock, 
            Symbol = "InvalidSymbol2", 
            Exchange = "Smart" };

        await client.Request.RequestMarketDataAsync(id, contract);

        object message = await task;

        Alert alert = Assert.IsType<Alert>(message);

        Assert.StartsWith("No security definition has been found for the request", alert.Message);
    }

    [Fact]
    public async Task BadResponseTestAsync()
    {
        Contract contract = new()
        {
            SecurityType = ContractSecurityType.Stock,
            Symbol = "IBM",
            Currency = "USD",
            Exchange = "SMART"
        };

        IInterReactClient client = await InterReactClient.CreateAsync(options =>
            options.LogFactory = LogFactory, TestContext.Current.CancellationToken);

        // This particular Id value will trigger a receive parse error when reading ContractDetails.
        int id = int.MaxValue;

        client.Response.Subscribe(x => { }, e => throw e);

        Task<ContractDetails> task = client
            .Response
            .WithRequestId(id)
            .OfType<ContractDetails>()
            .FirstAsync()
            .Timeout(TimeSpan.FromSeconds(6))
            .ToTask();

        await client.Request.RequestContractDetailsAsync(id, contract);

        await Assert.ThrowsAnyAsync<Exception>(() => task);
    }
}
