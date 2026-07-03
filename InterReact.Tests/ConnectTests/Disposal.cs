namespace ConnectTests;

public class Disposal(ITestOutputHelper output) : OutputHelperTestBase(output, LogLevel.Debug)
{
    [Fact]
    public async Task DisposalTestAsync()
    {
        IInterReactClient client = await InterReactClient.CreateAsync(options =>
            options.LogFactory = LogFactory, TestContext.Current.CancellationToken);

        await client.DisposeAsync();

        await Assert.ThrowsAnyAsync<Exception>(async() => await client.Request.RequestCurrentTimeAsync());
    }
}
