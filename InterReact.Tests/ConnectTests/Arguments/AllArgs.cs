using System.Diagnostics;
using System.Net;
using System.Reactive.Linq;

namespace Args;

public class AllArgs : ConnectTestBase
{
    public AllArgs(ITestOutputHelper output) : base(output) { }

    [Fact]
    public async Task AllArgsTest()
    {
        var connector = new InterReactClientConnector()
            .WithLogger(Logger)
            .WithIpAddress(IPAddress.IPv6Loopback)
            .WithClientId(111)
            .WithMaxServerVersion(ServerVersion.CASH_QTY)
            .WithMaxRequestsPerSecond(10);

        IInterReactClient client = await connector.ConnectAsync();
     
        await TestClient(client);
        Assert.Equal(IPAddress.IPv6Loopback, client.RemoteIPEndPoint.Address);
        Assert.Equal(111, connector.ClientId);
        Assert.Equal(ServerVersion.CASH_QTY, connector.ServerVersionMax);
        Assert.True(connector.ServerVersionCurrent >= InterReactClientConnector.ServerVersionMin);
        Assert.Equal(10, connector.MaxRequestsPerSecond);

        await client.DisposeAsync();
    }
}