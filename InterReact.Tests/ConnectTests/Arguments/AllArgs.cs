using System.Net;

namespace Args;

public class AllArgs(ITestOutputHelper output) : ConnectTestBase(output)
{
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