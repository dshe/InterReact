using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
namespace ConnectTests;

public class BadData(ITestOutputHelper output) : OutputHelperTestBase(output, LogLevel.Information)
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
            .ToTask();

        Contract contract = new() {
            SecurityType = ContractSecurityType.Stock, 
            Symbol = "InvalidSymbol2",
            Exchange = "Smart" };

        await client.Request.RequestMarketDataAsync(id, contract);

        object message = await task;

        Alert alert = Assert.IsType<Alert>(message);

        Assert.StartsWith("No security definition has been found for the request", alert.Message);

        await client.DisposeAsync();
    }

    [Fact]
    public async Task BadResponseTestAsync()
    {
        IInterReactClient client = await InterReactClient.CreateAsync(options =>
            options.LogFactory = LogFactory, TestContext.Current.CancellationToken);

        Contract contract = new()
        {
            SecurityType = ContractSecurityType.Stock,
            Symbol = "IBM",
            Currency = "USD",
            Exchange = "SMART"
        };

        // This particular Id value will trigger an error when reading ContractDetails.
        // The error will trigger OnError() on the Response observable.
        await client.Request.RequestContractDetailsAsync(int.MaxValue, contract);

        await Task.Delay(100, TestContext.Current.CancellationToken);

        await Assert.ThrowsAnyAsync<Exception>(async() => await client.Response);

        await client.DisposeAsync();
    }

}
