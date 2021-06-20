using System;
using System.Diagnostics;
using System.Net;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace InterReact.ConnectTests
{
    public class ConnectDefault : BaseConnectTest
    {
        public ConnectDefault(ITestOutputHelper output) : base(output) { }

        [Fact]
        public async Task Test()
        {
            var client = await new InterReactClientBuilder(Logger).BuildAsync();
            await TestClient(client);
            await client.DisposeAsync();
        }
    }

    public class ConnectIPv4Test : BaseConnectTest
    {
        public ConnectIPv4Test(ITestOutputHelper output) : base(output) { }

        [Fact]
        public async Task Test()
        {
            var client = await new InterReactClientBuilder(Logger).SetIpAddress(IPAddress.Loopback).BuildAsync();
            await TestClient(client);
            await client.DisposeAsync();
        }
    }

    public class ConnectIPv6Test : BaseConnectTest
    {
        public ConnectIPv6Test(ITestOutputHelper output) : base(output) { }

        [Fact]
        public async Task Test()
        {
            var client = await new InterReactClientBuilder(Logger).SetIpAddress(IPAddress.IPv6Loopback).BuildAsync();
            await TestClient(client);
            await client.DisposeAsync();
        }
    }

    public class ConnectArgumentsTest : BaseConnectTest
    {
        public ConnectArgumentsTest(ITestOutputHelper output) : base(output) { }

        [Fact]
        public async Task Test()
        {
            var client = await new InterReactClientBuilder(Logger)
                .SetIpAddress(IPAddress.IPv6Loopback)
                //.SetPort(7496) // default
                .SetClientId(111)
                .SetMaxRequestsPerSecond(123)
                .BuildAsync();
            await TestClient(client);
            Assert.Equal(IPAddress.IPv6Loopback, client.Request.Config.IPEndPoint.Address);
            //Assert.Equal(7496, client.Config.IPEndPoint.Port);
            Assert.Equal(111, client.Request.Config.ClientId);
            //Assert.True(client.Config.IsDemoAccount);
            Assert.NotEmpty(client.Request.Config.Date);
            Assert.NotEmpty(client.Request.Config.ManagedAccounts);
            Assert.True(client.Request.Config.ServerVersionCurrent >= Config.ServerVersionMin);
            await client.DisposeAsync();
        }

    }

    public class MessageSendRateDefaultTest : BaseConnectTest
    {
        public MessageSendRateDefaultTest(ITestOutputHelper output) : base(output) { }

        [Fact]
        public async Task Test()
        {
            var count = 0;
            var client = await new InterReactClientBuilder(Logger).BuildAsync();
            await Task.Delay(100); // warm up
            var start = Stopwatch.GetTimestamp();
            while (Stopwatch.GetTimestamp() - start < Stopwatch.Frequency)
            {
                client.Request.RequestGlobalCancel();
                count++;
            }
            Write($"message send rate: {count:0} messages/second.");
            Assert.InRange(count, 10, 100);

            await client.DisposeAsync();
        }

    }

    public class MessageSendRateChangeTest : BaseConnectTest
    {
        public MessageSendRateChangeTest(ITestOutputHelper output) : base(output) { }

        [Fact]
        public async Task Test()
        {
            var count = 0;
            var client = await new InterReactClientBuilder(Logger).SetMaxRequestsPerSecond(100).BuildAsync();

            await Task.Delay(100); // warm up
            var start = Stopwatch.GetTimestamp();
            while (Stopwatch.GetTimestamp() - start < Stopwatch.Frequency)
            {
                client.Request.RequestGlobalCancel();
                count++;
            }

            Write($"message send rate: {count:0} messages/second.");
            Assert.InRange(count, 0, 110);

            await client.DisposeAsync();
        }
    }

    public class DisposedTest : BaseConnectTest
    {
        public DisposedTest(ITestOutputHelper output) : base(output) { }

        [Fact]
        public async Task Test()
        {
            var client = await new InterReactClientBuilder(Logger).BuildAsync();
            await client.DisposeAsync();
            Assert.ThrowsAny<Exception>(() => client.Request.RequestCurrentTime());
            await Assert.ThrowsAnyAsync<Exception>(async () => await client.Services.CurrentTimeObservable);
        }
    }
}
