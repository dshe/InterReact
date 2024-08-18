using System.Net;

namespace Connection;

public class IpAddressTest(ITestOutputHelper output) : ConnectTestBase(output)
{
    [Fact]
    public async Task AllDefaultsTest()
    {
        IInterReactClient client = await InterReactClient.ConnectAsync();
        await client.DisposeAsync();
    }

    [Fact]
    public async Task IPv4Test()
    {
        IInterReactClient client = await InterReactClient.ConnectAsync(options =>
        {
            options.LogFactory = LogFactory;
            options.TwsIpAddress = IPAddress.Loopback;
        });

        await client.DisposeAsync();
    }

    [Fact]
    public async Task IPv6Test()
    {
        IInterReactClient client = await InterReactClient.ConnectAsync(options =>
        {
            options.LogFactory = LogFactory;
            options.TwsIpAddress = IPAddress.IPv6Loopback;
        });

        await client.DisposeAsync();
    }
}

