using RxSockets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace InterReact.UnitTests.Core
{
    public static class Exts
    {
        public static int? Nullify(this int i) => i == int.MaxValue ? null : i;
        public static double? Nullify(this double i) => i == double.MaxValue ? null : i;
        public static int UnNullify(this int? i) => i ?? int.MaxValue;
        public static double UnNullify(this double? i) => i ?? double.MaxValue;
    }

    public class Request_Message_Tests : UnitTestsBase
    {
        private readonly RequestMessage requestMessage;
        private readonly Limiter limiter = new();
        private string[] Strings = Array.Empty<string>();
        internal byte[] Bytes = Array.Empty<byte>();

        private void Send(byte[] bytes)
        {
            Bytes = bytes;
            var list = bytes
                .ToArraysFromBytesWithLengthPrefix()
                .ToStringArrays().ToList();
            Assert.Equal(1, list.Count);
            Strings = list[0];
        }

        public Request_Message_Tests(ITestOutputHelper output) : base(output)
        {
            requestMessage = new RequestMessage(Send, limiter);
        }

        private void AssertResult(params string?[]? strings)
        {
            var list = Strings;
            if (strings == null)
            {
                Assert.Equal(list.Length, 1);
                Assert.Equal(list[0], null);
            }
            else
            {
                Assert.Equal(list.Length, strings.Length);
                for (var i = 0; i < list.Length; i++)
                    Assert.Equal(list[i], strings[i]);
            }
        }

        [Fact]
        public void T01_Null()
        {
            requestMessage.Write(null).Send();
            AssertResult("");
        }

        [Fact]
        public void T02_Empty()
        {
            requestMessage.Write("").Send();
            AssertResult("");
        }

        [Fact]
        public void T03_Nulls()
        {
            requestMessage.Write(null).Write(null).Send();
            AssertResult("", "");
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

        [Fact]
        public void T07_NInt()
        {
            int? nint = null;
            requestMessage.Write(nint.EncodeNullable()).Send();
            AssertResult(int.MaxValue.ToString());
        }
    }
}
