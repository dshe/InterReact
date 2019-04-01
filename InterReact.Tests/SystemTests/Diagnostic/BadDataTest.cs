using System;
using System.IO;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using InterReact.Messages;
using Stringification;
using InterReact.Tests.Utility;
using Xunit;
using Xunit.Abstractions;
using Microsoft.Extensions.Logging;

namespace InterReact.Tests.SystemTests.Diagnostic
{
    public class BadDataTest : BaseSystemTest
    {
        public BadDataTest(SystemTestFixture fixture, ITestOutputHelper output) : base(fixture, output) { }

        [Fact]
        public async Task TestBadRequest()
        {
            var requestId = Client.Request.NextId();

            var alert = Client.Response
                .OfType<Alert>()
                .Where(r => r.RequestId == requestId)
                .FirstAsync()
                .Timeout(TimeSpan.FromSeconds(10))
                .ToTask();

            Client.Request.RequestMarketData(requestId, new Contract());

            await alert;
        }

        [Fact]
        public async Task TestBadResponse()
        {
            const int requestId = int.MaxValue;

            var task = Client.Response
                    .OfType<Exception>()
                    .FirstAsync()
                    .Timeout(TimeSpan.FromSeconds(10))
                    .ToTask();

            Client.Request.RequestMarketData(requestId, Stock1);

            var exception = await task;
            Logger.LogDebug(exception.Stringify());
            Assert.IsType<InvalidDataException>(exception);
        }

    }
}
