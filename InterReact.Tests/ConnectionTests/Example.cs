using System.Reactive.Linq;
namespace ConnectionTests;

public class Example(ITestOutputHelper output) : OutputHelperTestBase(output, LogLevel.Debug, "MyTest")
{
    public static readonly Contract Contract = new()
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

        await Task.Delay(2000, TestContext.Current.CancellationToken);

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

        await Task.Delay(2000, TestContext.Current.CancellationToken);

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
            .Take(2)
            .ToArray()
            .FirstAsync();

        Assert.Equal(5, messages.Length);

        await interReact.DisposeAsync();
    }

}

