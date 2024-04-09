using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;

namespace Contracts;

public class MatchingSymbolsAsync : CollectionTestBase
{
    public MatchingSymbolsAsync(ITestOutputHelper output, TestFixture fixture) : base(output, fixture) { }

    [Fact]
    public async Task GetMatchingSymbolsObservableTest()
    {
        string pattern = "X";

        IList<ContractDescription> descriptions = await Client.Service.FindMatchingSymbolsAsync(pattern, default);

        Assert.NotEmpty(descriptions);
    }

    [Fact]
    public async Task GetMatchingSymbolsObservableAlertTest()
    {
        string pattern = "";

        await Assert.ThrowsAsync<AlertException>(() => 
            Client.Service.FindMatchingSymbolsAsync(pattern, default));
    }
}
