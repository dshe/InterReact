using System;
using System.Diagnostics;
using System.Net;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
namespace InterReact.ConnectTests;

public class ConnectDefault : ConnectTestsBase
{
    public ConnectDefault(ITestOutputHelper output) : base(output) { }

    [Fact]
    public async Task Test()
    {
        var client = await new InterReactClientConnector().WithLogger(Logger).ConnectAsync();
        await TestClient(client);
        await client.DisposeAsync();
    }
}

public class ConnectIPv4Test : ConnectTestsBase
{
    public ConnectIPv4Test(ITestOutputHelper output) : base(output) { }

    [Fact]
    public async Task Test()
    {
        var client = await new InterReactClientConnector().WithLogger(Logger).WithIpAddress(IPAddress.Loopback).ConnectAsync();
        await TestClient(client);
        await client.DisposeAsync();
    }
}

public class ConnectIPv6Test : ConnectTestsBase
{
    public ConnectIPv6Test(ITestOutputHelper output) : base(output) { }

    [Fact]
    public async Task Test()
    {
        var client = await new InterReactClientConnector().WithLogger(Logger).WithIpAddress(IPAddress.IPv6Loopback).ConnectAsync();
        await TestClient(client);
        await client.DisposeAsync();
    }
}

public class ConnectArgumentsTest : ConnectTestsBase
{
    public ConnectArgumentsTest(ITestOutputHelper output) : base(output) { }

    [Fact]
    public async Task Test()
    {
        var client = await new InterReactClientConnector()
            .WithLogger(Logger)
            .WithIpAddress(IPAddress.IPv6Loopback)
            .WithClientId(111)
            .WithMaxRequestsPerSecond(123)
            .ConnectAsync();
        await TestClient(client);
        Assert.Equal(IPAddress.IPv6Loopback, client.Request.Builder.IPEndPoint.Address);
        Assert.Equal(111, client.Request.Builder.ClientId);
        Assert.True(client.Request.Builder.ServerVersionCurrent >= InterReactClientConnector.ServerVersionMin);
        await client.DisposeAsync();
    }

}

public class MessageSendRateDefaultTest : ConnectTestsBase
{
    public MessageSendRateDefaultTest(ITestOutputHelper output) : base(output) { }

    [Fact]
    public async Task Test()
    {
        var count = 0;
        var client = await new InterReactClientConnector().WithLogger(Logger).ConnectAsync();
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

public class MessageSendRateChangeTest : ConnectTestsBase
{
    public MessageSendRateChangeTest(ITestOutputHelper output) : base(output) { }

    [Fact]
    public async Task Test()
    {
        var count = 0;
        var client = await new InterReactClientConnector().WithLogger(Logger).WithMaxRequestsPerSecond(100).ConnectAsync();

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

public class DisposedTest : ConnectTestsBase
{
    public DisposedTest(ITestOutputHelper output) : base(output) { }

    [Fact]
    public async Task Test()
    {
        var client = await new InterReactClientConnector().WithLogger(Logger).ConnectAsync();
        await client.DisposeAsync();
        Assert.ThrowsAny<Exception>(() => client.Request.RequestCurrentTime());
        //await Assert.ThrowsAnyAsync<Exception>(async () => await client.Services.CurrentTimeObservable);
    }
}
