using System.Reactive.Linq;

namespace Other;

public class Disposal : ConnectTestBase
{
    public Disposal(ITestOutputHelper output) : base(output) { }

    [Fact]
    public async Task DisposalTest()
    {
        IInterReactClient client = await new InterReactClientConnector()
            .WithLoggerFactory(LogFactory)
            .ConnectAsync();
        
        await client.DisposeAsync();
        
        Assert.ThrowsAny<Exception>(() => client.Request.RequestCurrentTime());
        await Assert.ThrowsAnyAsync<Exception>(async () => await client.Service.GetMatchingSymbolsAsync("x"));
    }
}
