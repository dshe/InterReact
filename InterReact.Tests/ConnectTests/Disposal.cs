using System.Reactive.Linq;
namespace ConnectTests;

public class Disposal(ITestOutputHelper output) : OutputHelperTestBase(output, LogLevel.Debug)
{
    [Fact]
    public async Task DisposalTest()
    {
        IInterReactClient client = await InterReactClient.CreateAsync(options =>
            options.LogFactory = LogFactory, TestContext.Current.CancellationToken);

        await client.DisposeAsync();

        Assert.ThrowsAny<Exception>(() => client.Request.RequestCurrentTime());
        //await Assert.ThrowsAnyAsync<Exception>(() => client.Service.GetMatchingSymbolsAsync("x", default));
    }
}
