using System;
using System.Linq;
using System.IO;
using InterReact.Core;
using InterReact.Messages;
using InterReact.Tests.Utility;
using Xunit;
using Xunit.Abstractions;
using System.Collections.Generic;

namespace InterReact.Tests.UnitTests.Core
{
    public class ResponseReaderEmitTests : BaseUnitTest
    {
        public ResponseReaderEmitTests(ITestOutputHelper output) : base(output) {}

        private readonly List<object> messages = new List<object>();

        private object EmitMessage(string[] strings)
        {
            new ResponseComposer(new Config(), messages.Add).Compose(strings);
            return messages.LastOrDefault();
        }

        [Fact]
        public void T00_Null()
        {
            Assert.Throws<NullReferenceException>(() => EmitMessage(null));
        }

        [Fact]
        public void T01_Short_Message()
        {
            var e = Assert.Throws<InvalidDataException>(() => EmitMessage(new string[] { }));
            Assert.IsType<IndexOutOfRangeException>(e.InnerException);

            e = Assert.Throws<InvalidDataException>(() => EmitMessage(new[] { "1" })); // short
            Assert.IsType<IndexOutOfRangeException>(e.InnerException);

            e = Assert.Throws<InvalidDataException>(() => EmitMessage(new[] { "1", "2", "3" })); // short
            Assert.IsType<IndexOutOfRangeException>(e.InnerException);
        }

        [Fact]
        public void T02_Long_Message()
        {
            var longMessage = new[] {
                "1",  // code = price
                "99", // version
                "2",  // requestId
                "5",  // tickType = LastSize
                "9.9", // price
                "100", // size
                "0", // AutoExec, true
                "7" // extra
            };

            EmitMessage(longMessage);
            Assert.Equal(2, messages.Count);
            Assert.IsType<TickPrice>(messages[0]);
            var e = Assert.IsType<ResponseWarning>(messages[1]);
            Assert.StartsWith("Message longer", e.Message);
        }

        [Fact]
        public void T03_Undefined_Code()
        {
            var e = Assert.Throws<InvalidDataException>(() => EmitMessage(new string[] { null }));
            Assert.Equal("Undefined code ''.", e.InnerException.Message);

            e = Assert.Throws<InvalidDataException>(() => EmitMessage(new string[] { "notInSwitch" }));
            Assert.Equal("Undefined code 'notInSwitch'.", e.InnerException.Message);
        }

        [Fact]
        public void T04_ParseError()
        {
            var e = Assert.Throws<InvalidDataException>(() => EmitMessage(new[] { "1", "version", "cannotParseInt" }));
            Assert.IsType<InvalidDataException>(e.InnerException);
            Assert.StartsWith("Parse<", e.InnerException.Message);
        }

        [Fact]
        public void T03_Ok()
        {
            EmitMessage(new[] {
                "1",  // code = price
                "99", // version
                "2",  // requestId
                "5",  // tickType = LastSize
                "9.9", // price
                "100", // size
                "0" // AutoExec, true
            });
            Assert.IsType<TickPrice>(messages.Single());
        }
    }

}
