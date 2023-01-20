using System.Reactive.Linq;

namespace Orders;

public class Monitor : TestCollectionBase
{
    public Monitor(ITestOutputHelper output, TestFixture fixture) : base(output, fixture) { }

    [Fact]
    public async Task OrderMonitorTest()
    {
        if (!Client.RemoteIPEndPoint.Port.IsIBDemoPort())
            throw new Exception("Use demo account to place order.");

        var contract = new Contract();
        var order = new Order();

        OrderMonitor orderMonitor = Client.Service.PlaceOrder(order, contract);
  
        orderMonitor.MessagesObservable.Subscribe(x => Console.WriteLine(x));

        await Task.Delay(TimeSpan.FromSeconds(3));

        orderMonitor.Cancel();

        orderMonitor.Dispose();
    }
}
