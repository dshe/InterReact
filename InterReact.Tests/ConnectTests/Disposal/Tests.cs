using System.Reactive.Linq;

namespace Other;

public class Disposal : ConnectTestBase
{
    public Disposal(ITestOutputHelper output) : base(output) { }

    [Fact]
    public async Task DisposalTest()
    {
        IInterReactClient client = await InterReactClient.ConnectAsync(options =>
            options.LogFactory = LogFactory);

        await client.DisposeAsync();

        Assert.ThrowsAny<Exception>(() => client.Request.RequestCurrentTime());
        await Assert.ThrowsAnyAsync<Exception>(() => client.Service.FindMatchingSymbolsAsync("x", default));
    }
}
