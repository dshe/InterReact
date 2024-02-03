using Stringification;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;

namespace Account;

public class Positions : CollectionTestBase
{
    public Positions(ITestOutputHelper output, TestFixture fixture) : base(output, fixture) { }

    [Fact(Skip = "May interfere with PositionsObservableTest")]
    public async Task PositionsTest()
    {
        Task<IList<object>> task = Client
            .Response
            .Where(m => m is Position or PositionEnd)
            .Take(TimeSpan.FromMilliseconds(100))
            .ToList()
            .ToTask();

        Client.Request.RequestPositions();

        IList<object> positions = await task;

        // The account may or may not have positions.
        foreach (var p in positions)
            Write(p.Stringify());
    }

    [Fact]
    public async Task PositionsObservableTest()
    {
        IList<Position> positions = await Client
            .Service
            .PositionsObservable
            .Take(TimeSpan.FromMilliseconds(100))
            .ToList();

        // The account may or may not have positions.
        foreach (var p in positions)
            Write(p.Stringify());
    }

}
