using NodaTime;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;

namespace Orders;

public class Monitor : TestCollectionBase
{
    public Monitor(ITestOutputHelper output, TestFixture fixture) : base(output, fixture) { }

    [Fact]
    public async Task Test()
    {
        var contract = new Contract();
        var order = new Order();

        OrderMonitor orderMonitor = Client.Service.PlaceOrder(order, contract);
  
        orderMonitor.MessagesObservable.Subscribe(x => Console.WriteLine(x));

        await Task.Delay(TimeSpan.FromSeconds(3));

        orderMonitor.Cancel();

        ((IDisposable)orderMonitor).Dispose();
    }


}
