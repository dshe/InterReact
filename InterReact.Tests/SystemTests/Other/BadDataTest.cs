using System;
using System.IO;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace InterReact.SystemTests.Other;

public class BadDataTest : TestCollectionBase
{
    public BadDataTest(ITestOutputHelper output, TestFixture fixture) : base(output, fixture) { }

    [Fact]
    public async Task TestBadRequest()
    {
        int requestId = Client.Request.GetNextRequestId();

        Task<IHasRequestId> messageTask = Client.Response
            .OfType<IHasRequestId>()
            .Where(r => r.RequestId == requestId)
            .Timeout(TimeSpan.FromSeconds(5))
            .FirstAsync()
            .ToTask();

        Client.Request.RequestMarketData(requestId, new Contract());

        IHasRequestId message = await messageTask;
        Assert.IsType<Alert>(message);
    }

    [Fact]
    public async Task TestBadResponse()
    {
        int requestId = int.MaxValue; // this value will trigger a receive parse error

        IObservable<object> response = Client.Response;

        Client.Request.RequestContractDetails(requestId, StockContract1);

        await Assert.ThrowsAnyAsync<InvalidDataException>(async () => await response);
    }
}
