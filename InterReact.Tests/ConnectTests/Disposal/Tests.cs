using System.Reactive.Linq;

namespace Other;

public class Disposal : ConnectTestBase
{
    public Disposal(ITestOutputHelper output) : base(output) { }

    [Fact]
    public async Task DisposalTest()
    {
        var client = await new InterReactClientConnector().WithLogger(Logger).ConnectAsync();
        await client.DisposeAsync();
        Assert.ThrowsAny<Exception>(() => client.Request.RequestCurrentTime());
        await Assert.ThrowsAnyAsync<Exception>(async () => await client.Service.CurrentTimeObservable);
    }
}
