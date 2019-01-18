using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using InterReact.Core;
using InterReact.Tests.Utility;
using Xunit;
using Xunit.Abstractions;
using InterReact.Utility;

namespace InterReact.Tests.UnitTests.Core
{
    public class RequestMessageTests : BaseUnitTest
    {
        private readonly List<string> list = new List<string>();
        private readonly RequestMessage requestMessage;
        private readonly Limiter limiter = new Limiter();
        private readonly Action<byte[], int, int> sendBytes;

        public RequestMessageTests(ITestOutputHelper output) : base(output)
        {
            sendBytes = (bytes, offset, length) =>
            {
                var prefix = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(bytes, 0));
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
                        list.Add(end == start ? null : Encoding.UTF8.GetString(bytes, start, end - start));
                        start = ++end;
                    }
                }
            };
            requestMessage = new RequestMessage(sendBytes, limiter);
        }

        private void AssertResult(params string[] strings)
        {
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
