using Stringification;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;

namespace Other;

public class BadData : ConnectTestBase
{
    public BadData(ITestOutputHelper output) : base(output) { }

    [Fact]
    public async Task BadRequestTest()
    {
        IInterReactClient client = await new InterReactClientConnector()
            .WithLogger(Logger)
            .ConnectAsync();

        int requestId = client.Request.GetNextId();

        Task<IHasRequestId> task = client
            .Response
            .OfType<IHasRequestId>()
            .Where(r => r.RequestId == requestId)
            .Timeout(TimeSpan.FromSeconds(5))
            .FirstAsync()
            .ToTask();

        client.Request.RequestMarketData(requestId, new Contract());

        IHasRequestId message = await task;
        Assert.IsType<Alert>(message);
        
        Write(message.Stringify());
    }

    [Fact]
    public async Task BadResponseTest()
    {
        Contract StockContract1 = new()
        {
            SecurityType = SecurityType.Stock,
            Symbol = "IBM",
            Currency = "USD",
            Exchange = "SMART"
        };

        IInterReactClient client = await new InterReactClientConnector()
             .WithLogger(Logger)
             .ConnectAsync();

        // this particular value will trigger a receive parse error
        int requestId = int.MaxValue; 

        Task<ContractDetails> task = client
            .Response
            .OfType<ContractDetails>()
            .Where(x => x.RequestId == requestId)
            .FirstAsync()
            .ToTask();

        client.Request.RequestContractDetails(requestId, StockContract1);

        await Assert.ThrowsAnyAsync<Exception>(async () => await task);
    }
}
