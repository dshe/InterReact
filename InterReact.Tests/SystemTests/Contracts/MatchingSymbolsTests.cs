namespace Contracts;

public class MatchingSymbols(ITestOutputHelper output, TestFixture fixture) : CollectionTestBase(output, fixture)
{
    [Fact]
    public async Task MatchingSymbolsTest()
    {
        string pattern = "X";

        IList<ContractDescription> descriptions = 
            await Client.Service.GetMatchingSymbolsAsync(pattern);

        Assert.NotEmpty(descriptions);
    }

    [Fact]
    public async Task MatchingSymbolsErrorTest()
    {
        string pattern = "";

        await Assert.ThrowsAsync<AlertException>(() => 
            Client.Service.GetMatchingSymbolsAsync(pattern));
    }
}
