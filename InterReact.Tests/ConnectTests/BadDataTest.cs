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

        // This particular Id value will trigger a receive parse error when reading ContractDetails.
        int id = int.MaxValue;

        //client.Response.Subscribe(x => { }, e => throw e);

        //Task<object> task = client
        var task = client
            .Response
            //.WithRequestId(id)
            //.OfType<ContractDetails>()
            .OfType<Alert>()
            .FirstAsync()
            //.Timeout(TimeSpan.FromSeconds(3))
            .ToTask();

        //IObservable<Contract> obs = Observable.Throw<Contract>(new NullReferenceException());
        //await obs;

        await client.Request.RequestContractDetailsAsync(id, contract);

        Alert alert = await task;

        //Assert.IsType<Alert>();

        //Exception ex = await Assert.ThrowsAnyAsync<Exception>(() => task);

        //Write(ex.ToString());

        await Task.Delay(1000, TestContext.Current.CancellationToken);

        await client.DisposeAsync();

        await Task.Delay(1000, TestContext.Current.CancellationToken);

        // the error is not sent to the client !!!
    }

}
