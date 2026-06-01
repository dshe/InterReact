using System.Net;
namespace ConnectTests;

public class IpAddressTest(ITestOutputHelper output) : OutputHelperTestBase(output, LogLevel.Debug)
{
    [Fact]
    public async Task AllDefaultsTest()
    {
        IInterReactClient client = await InterReactClient.CreateAsync(null, TestContext.Current.CancellationToken);
        await client.DisposeAsync();
    }

    [Fact]
    public async Task IPv4Test()
    {
        IInterReactClient client = await InterReactClient.CreateAsync(options =>
        {
            options.LogFactory = LogFactory;
            options.TwsIpAddress = IPAddress.Loopback;
        }, TestContext.Current.CancellationToken);

        await client.DisposeAsync();
    }

    [Fact]
    public async Task IPv6Test()
    {
        IInterReactClient client = await InterReactClient.CreateAsync(options =>
        {
            options.LogFactory = LogFactory;
            options.TwsIpAddress = IPAddress.IPv6Loopback;
        }, TestContext.Current.CancellationToken);

        await client.DisposeAsync();
    }
}

