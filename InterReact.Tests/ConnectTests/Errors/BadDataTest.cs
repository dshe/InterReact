using Microsoft.Extensions.Logging;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;

namespace Other;

public class BadData : ConnectTestBase
{
    public BadData(ITestOutputHelper output) : base(output, LogLevel.Debug) { }

    [Fact]
    public async Task BadRequestTest()
    {
        IInterReactClient client = await new InterReactClientConnector()
            .WithLoggerFactory(LogFactory)
            .ConnectAsync();

        int id = client.Request.GetNextId();

        Task<IHasRequestId> task = client
            .Response
            .WithRequestId(id)
            .FirstAsync()
            .Timeout(TimeSpan.FromSeconds(6))
            .ToTask();

        client.Request.RequestMarketData(id, new Contract { Symbol = "InvalidSymbol2" });

        object message = await task;

        AlertMessage alert = Assert.IsType<AlertMessage>(message);

        Assert.StartsWith("Error validating request", alert.Message);
    }

    [Fact]
    public async Task BadResponseTest()
    {
        Contract contract = new()
        {
            SecurityType = SecurityType.Stock,
            Symbol = "IBM",
            Currency = "USD",
            Exchange = "SMART"
        };

        IInterReactClient client = await new InterReactClientConnector()
            .WithLoggerFactory(LogFactory)
            .ConnectAsync();

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

        await Assert.ThrowsAnyAsync<Exception>(async () => await task);
    }
}
