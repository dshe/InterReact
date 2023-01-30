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

        int id = client.Request.GetNextId();

        Task<object> task = client
            .Response
            .WithRequestId(id)
            .FirstAsync()
            .Timeout(TimeSpan.FromSeconds(10))
            .ToTask();

        client.Request.RequestMarketData(id, new Contract() { Symbol = "InvalidSymbol" });

        object message = await task;

        Write("Message: " + message.Stringify());

        Alert alert = Assert.IsType<Alert>(message);

        Assert.StartsWith("Error validating request", alert.Message);
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
        int id = int.MaxValue; 

        Task<ContractDetails> task = client
            .Response
            .WithRequestId(id)
            .OfType<ContractDetails>()
            .FirstAsync()
            .ToTask();

        client.Request.RequestContractDetails(id, StockContract1);

        await Assert.ThrowsAnyAsync<Exception>(async () => await task);
    }
}
