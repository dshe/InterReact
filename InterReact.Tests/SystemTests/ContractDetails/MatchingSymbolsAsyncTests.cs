namespace Contracts;

public class MatchingSymbolsAsync : TestCollectionBase
{
    public MatchingSymbolsAsync(ITestOutputHelper output, TestFixture fixture) : base(output, fixture) { }

    [Fact]
    public async Task Test()
    {
        string pattern = "X";

        IList<IHasRequestId> messages = await Client
            .Service
            .GetMatchingSymbolsAsync(pattern);

        Assert.Empty(messages.OfType<AlertMessage>());

        SymbolSamples samples = messages.OfType<SymbolSamples>().Single();

        IList<ContractDescription> descriptions = samples.Descriptions;
    }

}
