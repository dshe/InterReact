using System.Reactive.Linq;
namespace Temp;

public sealed class ObservableTests2(ITestOutputHelper output) : OutputHelperTestBase(output)
{
       private IObservable<string> CreateObservable()
       {
            return Observable.Create<string>(async (obs, ct) =>
            {
                Write("Subscribing");

                //ct.Register(() => Write("Unsubscribing"));

                while (!ct.IsCancellationRequested)
                {
                    obs.OnNext("!");
                    await Task.Delay(100, ct);
                }
                obs.OnCompleted();
            });
    }

    [Fact]
    public async Task CancellationAsync()
    {
        var cts = new CancellationTokenSource();
        cts.CancelAfter(200);

        var observable = CreateObservable().WithTimeout(TimeSpan.FromSeconds(1), cts.Token);

        await Assert.ThrowsAsync<OperationCanceledException>(async () => await observable);
        //string[] xx = await observable.Take(3).ToArray();

       // observable.Subscribe(x => Write(x), 
       //     ex => Write(ex.Message), 
       //     () => Write("Completed"));
    }

}
