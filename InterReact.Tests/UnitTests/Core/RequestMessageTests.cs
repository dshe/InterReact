using RxSockets;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace InterReact.UnitTests.Core;

public sealed class NullRxSocketClient : IRxSocketClient
{
    public IPEndPoint RemoteIPEndPoint => throw new NotImplementedException();
    public bool Connected => throw new NotImplementedException();
    public ValueTask DisposeAsync() => ValueTask.CompletedTask;
    public IAsyncEnumerable<byte> ReceiveAllAsync() => throw new NotImplementedException();
    public int Send(ReadOnlySpan<byte> buffer) => throw new NotImplementedException();
}

public class Request_Message_Tests : UnitTestsBase
{
    private readonly RequestMessage requestMessage = 
        new(new NullRxSocketClient(), new RingLimiter());
    public Request_Message_Tests(ITestOutputHelper output) : base(output) {}

    private void AssertResult(params string[] strings)
    {
        List<string> messageStrings = requestMessage.Strings;
        Assert.Equal(messageStrings.Count, strings.Length);
        for (var i = 0; i < messageStrings.Count; i++)
            Assert.Equal(messageStrings[i], strings[i]);
        messageStrings.Clear();
    }

    [Fact]
    public void T01_Null()
    {
        requestMessage.Write(null);
        AssertResult("");
    }

    [Fact]
    public void T02_Empty()
    {
        requestMessage.Write("");
        AssertResult("");
    }

    [Fact]
    public void T03_Nulls()
    {
        requestMessage.Write(null).Write(null);
        AssertResult("", "");
    }

    [Fact]
    public void T04_String()
    {
        requestMessage.Write("test");
        AssertResult("test");
    }

    [Fact]
    public void T05_Strings()
    {
        requestMessage.Write("test1").Write("test2");
        AssertResult("test1", "test2");
    }

    [Fact]
    public void T06_Int()
    {
        requestMessage.Write(42);
        AssertResult("42");
    }

    [Fact]
    public void T07_NInt()
    {
        int? nint = null;
        requestMessage.Write(nint.EncodeNullable());
        AssertResult(int.MaxValue.ToString());
    }
}
