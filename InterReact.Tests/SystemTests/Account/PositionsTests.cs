using Stringification;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;

namespace Account;

public class Positions : TestCollectionBase
{
    public Positions(ITestOutputHelper output, TestFixture fixture) : base(output, fixture) { }

    [Fact]
    public async Task PositionsTest()
    {
        Task<IList<Position>> task = Client
            .Response
            .Where(x => x is Position || x is PositionEnd)
            .TakeWhile(o => o is not PositionEnd)
            .Cast<Position>()
            .ToList()
            .ToTask();

        Client.Request.RequestPositions();

        IList<Position> list = await task;

        Client.Request.CancelPositions();

        // The account may or may not have positions.
        if (!list.Any())
            Write("no positions!");
        foreach (object o in list)
            Write(o.Stringify());
    }

    [Fact]
    public async Task PositionsServiceTest()
    {
        IList<Position> list = await Client
            .Service
            .PositionsObservable
            .TakeWhile(o => o is not PositionEnd)
            .Cast<Position>()
            .ToList();
 
        // The account may or may not have positions.
        if (!list.Any())
            Write("no positions!");
        foreach (object o in list)
            Write(o.Stringify());
    }
}
