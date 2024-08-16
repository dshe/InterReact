using Microsoft.Extensions.Logging;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;

namespace Other;

public class BadData(ITestOutputHelper output) : ConnectTestBase(output, LogLevel.Debug)
{
    [Fact]
    public async Task BadRequestTest()
    {
        IInterReactClient client = await InterReactClient.ConnectAsync(options =>
            options.LogFactory = LogFactory);

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

        client.Request.RequestMarketData(id, contract);

        object message = await task;

        AlertMessage alert = Assert.IsType<AlertMessage>(message);

        Assert.StartsWith("No security definition has been found for the request", alert.Message);
    }

    [Fact]
    public async Task BadResponseTest()
    {
        Contract contract = new()
        {
            SecurityType = ContractSecurityType.Stock,
            Symbol = "IBM",
            Currency = "USD",
            Exchange = "SMART"
        };

        IInterReactClient client = await InterReactClient.ConnectAsync(options =>
            options.LogFactory = LogFactory);

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

        client.Request.RequestContractDetails(id, contract);

        await Assert.ThrowsAnyAsync<Exception>(() => task);
    }
}
