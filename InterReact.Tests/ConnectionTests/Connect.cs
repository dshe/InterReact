using System.Net;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
namespace ConnectionTests;

public class Connect(ITestOutputHelper output) : OutputHelperTestBase(output, LogLevel.Debug)
{
    [Fact]
    public async Task AllDefaultsTestAsync()
    {
        IInterReactClient client = await InterReactClient.CreateAsync(null, TestContext.Current.CancellationToken);
        await client.DisposeAsync();
    }

    [Fact]
    public async Task IPv4TestAsync()
    {
        IInterReactClient client = await InterReactClient.CreateAsync(options =>
        {
            options.LogFactory = LogFactory;
            options.TwsIpAddress = IPAddress.Loopback;
        }, TestContext.Current.CancellationToken);

        await client.DisposeAsync();
    }

    [Fact]
    public async Task IPv6TestAsync()
    {
        IInterReactClient client = await InterReactClient.CreateAsync(options =>
        {
            options.LogFactory = LogFactory;
            options.TwsIpAddress = IPAddress.IPv6Loopback;
        }, TestContext.Current.CancellationToken);

        await client.DisposeAsync();
    }

    /// <summary>
    /// Login arms a three second timeout for data receive.
    /// Make sure cancellation does not occur on successful creation of client.
    /// </summary>
    [Fact]
    public async Task SurvivesLoginTimerTestAsync()
    {
        IInterReactClient client = await InterReactClient.CreateAsync(options =>
            options.LogFactory = LogFactory, TestContext.Current.CancellationToken);

        try
        {
            await Task.Delay(TimeSpan.FromSeconds(5), TestContext.Current.CancellationToken);

            IList<string> accounts = await client.Service.GetManagedAccountsAsync(TestContext.Current.CancellationToken);

            Assert.NotEmpty(accounts);
        }
        finally
        {
            await client.DisposeAsync();
        }
    }
}

