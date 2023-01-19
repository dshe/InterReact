using System.Net;

namespace Args;

public class IpAddress : ConnectTestBase
{
    public IpAddress(ITestOutputHelper output) : base(output) { }

    [Fact]
    public async Task DefaultsTest()
    {
        var client = await new InterReactClientConnector()
            .WithLogger(Logger)
            .ConnectAsync();

        await TestClient(client);

        await client.DisposeAsync();
    }

    [Fact]
    public async Task IPv4Test()
    {
        var client = await new InterReactClientConnector()
            .WithLogger(Logger)
            .WithIpAddress(IPAddress.Loopback)
            .ConnectAsync();

        await TestClient(client);

        await client.DisposeAsync();
    }

    [Fact]
    public async Task IPv6Test()
    {
        var client = await new InterReactClientConnector()
            .WithLogger(Logger)
            .WithIpAddress(IPAddress.IPv6Loopback)
            .ConnectAsync();

        await TestClient(client);

        await client.DisposeAsync();
    }
}

