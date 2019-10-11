using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;
using InterReact.Tests.Utility;
using Xunit;
using Xunit.Abstractions;
using System.Threading;
using System.Net.Sockets;
using RxSockets;

namespace InterReact.Tests.UnitTests
{
    public class TestClientNoConnect : BaseUnitTest
    {
        public TestClientNoConnect(ITestOutputHelper output) : base(output) {}

        [Fact]
        public async Task T01_Cancel()
        {
            var ex = await Assert.ThrowsAnyAsync<OperationCanceledException>(async () =>
                await new InterReactClientBuilder(LoggerFactory).BuildAsync(ct: new CancellationToken(true)));
            Logger.LogDebug(ex.Message);
        }

        [Fact]
        public async Task T02_Timeout()
        {
            var cts = new CancellationTokenSource(0);
            var ex = await Assert.ThrowsAsync<Exception>(async () => await new InterReactClientBuilder(LoggerFactory).BuildAsync(1));
            Logger.LogDebug(ex.Message);
        }

        [Fact]
        public async Task T03_ConnectionRefused()
        {
            var task = new InterReactClientBuilder(LoggerFactory).SetPort(999).BuildAsync();
            var ex = await Assert.ThrowsAsync<Exception> (async () => await task);
            Logger.LogDebug(ex.Message);
        }

    }
}
