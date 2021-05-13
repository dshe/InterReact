using System;
using System.Linq;
using System.IO;
using Xunit;
using Xunit.Abstractions;
using System.Collections.Generic;

namespace InterReact.UnitTests.Core
{
    public class ResponseComposerTests : BaseTest
    {
        public ResponseComposerTests(ITestOutputHelper output) : base(output) { }

        private readonly List<object> messages = new List<object>();

        private void Compose(string[] strings)
        {
            var rc = new ResponseComposer(new Config(), Logger);
            rc.Compose(strings, m => messages.Add(m));
        }

        [Fact]
        public void T00_Null()
        {
            Assert.Throws<ArgumentNullException>(() => Compose(null!));
        }

        [Fact]
        public void T01_Short_Message()
        {
            var e = Assert.Throws<InvalidDataException>(() => Compose(new string[] { }));
            Assert.IsType<IndexOutOfRangeException>(e.InnerException);

            e = Assert.Throws<InvalidDataException>(() => Compose(new[] { "1", "2" })); // short
            Assert.IsType<IndexOutOfRangeException>(e.InnerException);
        }

        [Fact]
        public void T02_Long_Message()
        {
            var longMessage = new[] {
                "1",  // code = TickPrice
                "99", // version
                "21",  // requestId
                "5",  // tickType = LastSize
                "9.9", // price
                "100", // size
                "1", // AutoExec, true
                "too long", // extra, to make it too long
            };
            Assert.Throws<InvalidDataException>(() => Compose(longMessage));
        }

        [Fact]
        public void T03_Undefined_Code()
        {
            var e = Assert.Throws<InvalidDataException>(() => Compose(new string[] { "" }));
            Assert.Equal("Undefined code ''.", e.InnerException?.Message);

            e = Assert.Throws<InvalidDataException>(() => Compose(new string[] { "notInSwitch" }));
            Assert.Equal("Undefined code 'notInSwitch'.", e.InnerException?.Message);
        }

        [Fact]
        public void T04_ParseError()
        {
            var e = Assert.Throws<InvalidDataException>(() => Compose(new[] { "1", "version", "cannotParseInt" }));
            Assert.IsType<ArgumentException>(e.InnerException);
            Assert.StartsWith("Parse", e.InnerException?.Message);
        }

        [Fact]
        public void T03_Ok()
        {
            Compose(new[] {
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
