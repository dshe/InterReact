using System.Reactive.Linq;
namespace ConnectTests;

public class SimplestExample(ITestOutputHelper output) : OutputHelperTestBase(output, LogLevel.Debug, "MyTest")
{
    public readonly Contract Contract = new()
    {
        SecurityType = ContractSecurityType.Cash,
        Symbol = "EUR",
        Currency = "USD",
        Exchange = "IDEALPRO"
    };

    [Fact]
    public async Task Test1Async()
    {
        IInterReactClient client = await InterReactClient.CreateAsync(
            x => x.LogFactory = LogFactory, TestContext.Current.CancellationToken);

        int id = client.Request.GetNextId();

        IDisposable subscription = client
            .Response
            .WithRequestId(id)
            .OfType<PriceTick>()
            .Subscribe(onNext: priceTick => Write($"Price = {priceTick.Price}"));

        await client.Request.RequestMarketDataAsync(id, Contract);

        await Task.Delay(3000, TestContext.Current.CancellationToken);

        subscription.Dispose();

        await client.DisposeAsync();
    }


    [Fact]
    public async Task Test2Async()
    {
        IInterReactClient interReact = await InterReactClient.CreateAsync(
            x => x.LogFactory = LogFactory, TestContext.Current.CancellationToken);

        IDisposable subscription = interReact
            .Service
            .CreateMarketDataObservable(Contract)
            .OfTickClass(selector => selector.PriceTick)
            .Subscribe(onNext: priceTick => Write($"Price = {priceTick.Price}"));

        await Task.Delay(3000, TestContext.Current.CancellationToken);

        subscription.Dispose();

        await interReact.DisposeAsync();
    }

    [Fact]
    public async Task Test3Async()
    {
        IInterReactClient interReact = await InterReactClient.CreateAsync(
            x => x.LogFactory = LogFactory, TestContext.Current.CancellationToken);

        IHasRequestId[] messages = await interReact
            .Service
            .CreateMarketDataObservable(Contract)
            .Take(8)
            .ToArray()
            .FirstAsync();

        Assert.Equal(8, messages.Length);

        await interReact.DisposeAsync();
    }

    /*
    Logger.LogCritical("Logger.LogCritical");
    Logger.LogError("Logger.LogError");
    Logger.LogWarning("Logger.LogWarning");
    Logger.LogInformation("Logger.LogInformation");
    Logger.LogDebug("Logger.LogDebug");
    Logger.LogTrace("Logger.LogTrace");
    */
}

