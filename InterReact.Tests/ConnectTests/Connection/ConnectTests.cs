using System.Net;

namespace Args;

public class IpAddress : ConnectTestBase
{
    public IpAddress(ITestOutputHelper output) : base(output) { }

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
            options.TwsIpAddress = IPAddress.Loopback.ToString();
        });

        await client.DisposeAsync();
    }

    [Fact]
    public async Task IPv6Test()
    {
        IInterReactClient client = await InterReactClient.ConnectAsync(options =>
        {
            options.LogFactory = LogFactory;
            options.TwsIpAddress = IPAddress.IPv6Loopback.ToString();
        });

        await client.DisposeAsync();
    }
}

