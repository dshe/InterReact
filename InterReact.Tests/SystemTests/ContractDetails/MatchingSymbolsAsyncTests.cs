using System.Reactive.Joins;
using System.Reactive.Linq;

namespace Contracts;

public class MatchingSymbolsAsync : TestCollectionBase
{
    public MatchingSymbolsAsync(ITestOutputHelper output, TestFixture fixture) : base(output, fixture) { }

    [Fact]
    public async Task Test()
    {
        string pattern = "IBM";

        SymbolSamples samples = await Client
            .Service
            .GetMatchingSymbolsAsync(pattern);

        await Task.Delay(2);
    }

}
