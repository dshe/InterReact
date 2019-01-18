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
                await InterReactClient.BuildAsync(timeout: -1, ct: new CancellationToken(true)));
            Write(ex.Message);
        }

        [Fact]
        public async Task T02_Timeout()
        {
            var ex = await Assert.ThrowsAsync<TimeoutException>(async () => await InterReactClient.BuildAsync(timeout: 0));
            Write(ex.Message);
        }

        [Fact]
        public async Task T03_ConnectionRefused()
        {
            var task = InterReactClient
                .Builder
                .SetPort(NetworkHelper.GetRandomAvailablePort())
                .BuildAsync(timeout: -1);
            var ex = await Assert.ThrowsAsync<IOException>(async () => await task);
            Assert.StartsWith("ConnectionRefused", ex.Message);
            Write(ex.Message);
            Write(ex.InnerException.Message);
        }

        [Fact]
        public async Task T04_NoResponse()
        {
            var serverSocket = NetworkHelper.CreateSocket();
            var endPoint = NetworkHelper.GetEndPointOnLoopbackRandomPort();
            serverSocket.Bind(endPoint);
            serverSocket.Listen(10);

            var task = InterReactClient
                .Builder
                .SetPort(endPoint.Port)
                .BuildAsync(timeout: 1000);
            var ex = await Assert.ThrowsAsync<TimeoutException>(async () => await task);
            Assert.StartsWith("No response", ex.Message);
            Write(ex.Message);
        }
    }
}
