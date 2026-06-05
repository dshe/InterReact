using System.Reactive.Linq;
namespace SystemTests;

public class ContractDetail(ITestOutputHelper output, TestFixture fixture) : CollectionTestBase(output, fixture)
{
    [Fact]
    public async Task ContractDetailObservableTestAsync()
    {
        Contract contract = new()
        {
            SecurityType = ContractSecurityType.Stock,
            Symbol = "C",
            Currency = "USD"
        };

        ContractDetails[] cds = await Client
            .Service
            .CreateContractDetailsObservable(contract)
            .OfTypeOnly<ContractDetails>()
            .WithTimeout(TimeSpan.FromSeconds(5), TestContext.Current.CancellationToken)
            .Catch<ContractDetails, Exception>(ex =>
            {
                // Could be symbol not found, timout or other error
                Write(ex.Message);
                return Observable.Empty<ContractDetails>();
            })
           .ToArray();

        Assert.True(cds.Length > 1); // multiple exchanges

        foreach (var cd in cds)
            Write(cd?.Stringify() ?? "");
    }

    [Fact]
    public async Task ContractDetailObservablePushTestAsync()
    {
        Write("Start");

        Contract contract = new()
        {
            SecurityType = ContractSecurityType.Stock,
            Symbol = "X",
            Currency = "USD"
        };

        IDisposable subscription = Client
            .Service
            .CreateContractDetailsObservable(contract)
            .Timeout(TimeSpan.FromSeconds(5))
            .Subscribe(
                onNext: m => Write(m?.Stringify() ?? ""),
                onError: ex => Write(ex?.Stringify() ?? ""),
                onCompleted: () => Write("Completed")
            );

        await Task.Delay(TimeSpan.FromSeconds(2), TestContext.Current.CancellationToken);

        subscription.Dispose();
    }

    [Fact]
    public async Task ContractDetailErrorTestAsync()
    {
        Contract contract = new()
        {
            SecurityType = ContractSecurityType.Stock
        };

        var ex = await Assert.ThrowsAsync<AlertException>(async () => await Client
            .Service
            .GetContractDetailsAsync(contract));

        Write(ex.Message);
    }

    [Fact]
    public async Task ContractDetailTimeoutTestAsync()
    {
        Contract contract = new()
        {
            SecurityType = ContractSecurityType.Stock,
            Symbol = "A",
            Currency = "USD"
        };

        await Assert.ThrowsAsync<TimeoutException>(async() => await Client
            .Service
            .CreateContractDetailsObservable(contract)
            .Timeout(TimeSpan.FromMilliseconds(1))
            .ToList());

     }
}
