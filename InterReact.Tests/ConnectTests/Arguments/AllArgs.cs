using System.Net;

namespace Args;

public class AllArgs : ConnectTestBase
{
    public AllArgs(ITestOutputHelper output) : base(output) { }

    [Fact]
    public async Task AllArgsTest()
    {
        IInterReactClient client = await InterReactClient.ConnectAsync(options =>
        {
            options.LogFactory = LogFactory;
            options.TwsIpAddress = IPAddress.IPv6Loopback.ToString();
            options.TwsClientId = 1234;
            options.MaxRequestsPerSecond = 10;
            options.UseDelayedTicks = false;
        });

        Assert.Equal(IPAddress.IPv6Loopback, client.RemoteIpEndPoint.Address);

        await client.DisposeAsync();
    }
}