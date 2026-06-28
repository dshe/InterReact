using System.Net;
namespace ConnectTests;

public class AllArgs(ITestOutputHelper output) : OutputHelperTestBase(output)
{
    [Fact]
    public async Task AllArgsTestAsync()
    {
        IInterReactClient client = await InterReactClient.CreateAsync(options =>
        {
            options.LogFactory = LogFactory;
            options.TwsIpAddress = IPAddress.IPv6Loopback;
            options.TwsClientId = 1234;
            options.UseDelayedTicks = false;
        }, TestContext.Current.CancellationToken);

        Assert.Equal(IPAddress.IPv6Loopback, client.RemoteIpEndPoint.Address);

        await client.DisposeAsync();
    }
}
