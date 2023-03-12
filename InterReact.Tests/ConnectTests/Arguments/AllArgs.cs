using System.Net;

namespace Args;

public class AllArgs : ConnectTestBase
{
    public AllArgs(ITestOutputHelper output) : base(output) { }

    [Fact]
    public async Task AllArgsTest()
    {
        InterReactClientConnector connector = new InterReactClientConnector()
            .WithLoggerFactory(LogFactory)
            .WithIpAddress(IPAddress.IPv6Loopback)
            .WithClientId(1234)
            .WithMaxRequestsPerSecond(10)
            .DoNotUseDelayedTicks();

        IInterReactClient client = await connector.ConnectAsync();
     
        Assert.Equal(IPAddress.IPv6Loopback, client.Connection.RemoteIpEndPoint.Address);
        Assert.Equal(1234, client.Connection.ClientId);

        await client.DisposeAsync();
    }
}