using RxSockets;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace InterReact.UnitTests.Core
{
    public class RxSocketTestClient : IRxSocketClient
    {
        public bool Connected => throw new NotImplementedException();
        public ValueTask DisposeAsync() => throw new NotImplementedException();
        public IAsyncEnumerable<byte> ReceiveAllAsync() => throw new NotImplementedException();
        public readonly List<string?> List = new();
        public void Send(ReadOnlySpan<byte> bytes)
        {
            var length = bytes.Length;
            var prefix = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(bytes.ToArray(), 0));
            Assert.Equal(length, prefix + 4);
            Assert.True(bytes[length - 1] == 0);
            var start = 4;
            var end = 4;
            while (end < length)
            {
                if (bytes[end] != 0)
                    end++;
                else
                {
                    List.Add(end == start ? null : Encoding.UTF8.GetString(bytes.ToArray(), start, end - start));
                    start = ++end;
                }
            }
        }


    }

    public class Request_Message_Tests : BaseUnitTest
    {
        private readonly RxSocketTestClient RxSocketTestClient = new();
        private readonly RequestMessage requestMessage;
        private readonly Limiter limiter = new();

        public Request_Message_Tests(ITestOutputHelper output) : base(output)
        {
            requestMessage = new RequestMessage(RxSocketTestClient, limiter);
        }

        private void AssertResult(params string?[]? strings)
        {
            var list = RxSocketTestClient.List;
            if (strings == null)
            {
                Assert.Equal(list.Count, 1);
                Assert.Equal(list[0], null);
            }
            else
            {
                Assert.Equal(list.Count, strings.Length);
                for (var i = 0; i < list.Count; i++)
                    Assert.Equal(list[i], strings[i]);
            }
        }

        [Fact]
        public void T01_Null()
        {
            requestMessage.Write(null).Send();
            AssertResult(null);
        }

        [Fact]
        public void T02_Empty()
        {
            requestMessage.Write("").Send();
            AssertResult(null);
        }

        [Fact]
        public void T03_Nulls()
        {
            requestMessage.Write(null).Write(null).Send();
            AssertResult(null, null);
        }

        [Fact]
        public void T04_String()
        {
            requestMessage.Write("test").Send();
            AssertResult("test");
        }

        [Fact]
        public void T05_Strings()
        {
            requestMessage.Write("test1").Write("test2").Send();
            AssertResult("test1", "test2");
        }

        [Fact]
        public void T06_Int()
        {
            requestMessage.Write(42).Send();
            AssertResult("42");
        }
    }
}
