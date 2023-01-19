using System.Reactive.Linq;

namespace Orders;

public class Monitor : TestCollectionBase
{
    public Monitor(ITestOutputHelper output, TestFixture fixture) : base(output, fixture) { }

    [Fact]
    public async Task Test()
    {
        if (!Client.RemoteIPEndPoint.Port.IsIBDemoPort())
            throw new Exception("Cannot place order. Not the demo account");

        var contract = new Contract();
        var order = new Order();

        OrderMonitor orderMonitor = Client.Service.PlaceOrder(order, contract);
  
        orderMonitor.MessagesObservable.Subscribe(x => Console.WriteLine(x));

        await Task.Delay(TimeSpan.FromSeconds(3));

        orderMonitor.Cancel();

        orderMonitor.Dispose();
    }
}
