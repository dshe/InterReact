using System.Net;

namespace Args;

public class IpAddress : ConnectTestBase
{
    public IpAddress(ITestOutputHelper output) : base(output) { }

    [Fact]
    public async Task AllDefaultsTest()
    {
        IInterReactClient client = await new InterReactClientConnector()
            .ConnectAsync();

        await client.DisposeAsync();
    }

    [Fact]
    public async Task IPv4Test()
    {
        IInterReactClient client = await new InterReactClientConnector()
            .WithLoggerFactory(LogFactory)
            .WithIpAddress(IPAddress.Loopback)
            .ConnectAsync();

        await client.DisposeAsync();
    }

    [Fact]
    public async Task IPv6Test()
    {
        IInterReactClient client = await new InterReactClientConnector()
            .WithLoggerFactory(LogFactory)
            .WithIpAddress(IPAddress.IPv6Loopback)
            .ConnectAsync();

        await client.DisposeAsync();
    }
}

