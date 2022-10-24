using NodaTime;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace InterReact.SystemTests.Other;

public class TimeServiceTests : TestCollectionBase
{
    public TimeServiceTests(ITestOutputHelper output, TestFixture fixture) : base(output, fixture) { }

    [Fact]
    public async Task TestTime()
    {
        Instant dt = await Client.Service.CurrentTimeObservable;

        Write($"Time: {dt}.");
    }
}
