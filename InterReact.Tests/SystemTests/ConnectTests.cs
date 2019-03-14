using System;
using System.Diagnostics;
using System.Net;
using System.Reactive.Linq;
using System.Threading.Tasks;
using InterReact;
using InterReact.Extensions;
using Stringification;
using InterReact.Core;
using Xunit;
using Xunit.Abstractions;
using NodaTime;
using NodaTime.Text;

namespace InterReact.Tests.SystemTests
{
    [Collection("SystemTestCollection")]
    public class ConnectTests
    {
        protected readonly Action<string> Write;

        public ConnectTests(ITestOutputHelper output)
            => Write = output.WriteLine;

        private async Task TestClient(IInterReactClient client)
        {
            client.Response.Stringify().Subscribe(Write);
            var dt = await client.Services.CurrentTimeObservable;
            Write($"Instant: {dt}");
        }

        [Fact]
        public async Task T00_ConnectDefault()
        {
            IInterReactClient client = await InterReactClient.Builder.BuildAsync();
            await TestClient(client);
            await client.DisconnectAsync();
        }

        [Fact]
        public async Task T01_ConnectIPv4()
        {
            using (var client = await InterReactClient.Builder.SetIpAddress(IPAddress.Loopback).BuildAsync())
                await TestClient(client);
        }

        [Fact]
        public async Task T02_ConnectIPv6()
        {
            using (var client = await InterReactClient.Builder.SetIpAddress(IPAddress.IPv6Loopback).BuildAsync())
                await TestClient(client);
        }

        [Fact]
        public async Task T03_ConnectArguments()
        {
            using (var client = await InterReactClient
                .Builder
                .SetIpAddress(IPAddress.IPv6Loopback)
                .SetPort(7497)
                .SetClientId(111)
                .SetMaxRequestsPerSecond(123)
                .BuildAsync())
            {
                await TestClient(client);
                Assert.Equal(IPAddress.IPv6Loopback, client.Config.IPEndPoint.Address);
                Assert.Equal(7497, client.Config.IPEndPoint.Port);
                Assert.Equal(111, client.Config.ClientId);
                
                Assert.True(client.Config.IsDemoAccount);
                Assert.NotEmpty(client.Config.Date);
                Assert.NotEmpty(client.Config.ManagedAccounts);
                Assert.True(client.Config.ServerVersionCurrent >= Config.ServerVersionMin);
            }
        }

        [Fact]
        public async Task T04_MessageSendRateDefault()
        {
            var count = 0;
            using (var client = await InterReactClient.Builder.BuildAsync())
            {
                await Task.Delay(500); // warm up
                var start = Stopwatch.GetTimestamp();
                while (Stopwatch.GetTimestamp() - start < Stopwatch.Frequency)
                {
                    client.Request.RequestGlobalCancel();
                    count++;
                }
            }
            Write($"message send rate: {count:0} messages/second.");
            Assert.InRange(count, 10, 100);
        }

        [Fact]
        public async Task T05_MessageSendRateChange()
        {
            var count = 0;
            using (var client = await InterReactClient.Builder.SetMaxRequestsPerSecond(100).BuildAsync())
            {
                await Task.Delay(500); // warm up
                var start = Stopwatch.GetTimestamp();
                while (Stopwatch.GetTimestamp() - start < Stopwatch.Frequency)
                {
                    client.Request.RequestGlobalCancel();
                    count++;
                }
            }
            Write($"message send rate: {count:0} messages/second.");
            Assert.InRange(count, 0, 110);
        }

        [Fact]
        public async Task T06_Disposed()
        {
            var client = await InterReactClient.Builder.BuildAsync();
            client.Dispose();
            Assert.Throws<ObjectDisposedException>(() => client.Request.RequestCurrentTime());
            await Assert.ThrowsAsync<ObjectDisposedException>(async () => await client.Services.CurrentTimeObservable);
        }
    }
}
