using System;
using System.Linq;
using InterReact.Core;
using InterReact.Tests.Utility;
using System.IO;
using Xunit;
using Xunit.Abstractions;
using System.Collections.Generic;
using NodaTime;
using NodaTime.Text;

namespace InterReact.Tests.UnitTests.Core
{
    public class ResponseReaderParseTests : BaseUnitTest
    {
        private readonly List<object> alerts = new List<object>();
        private readonly ResponseComposer reader;
        public ResponseReaderParseTests(ITestOutputHelper output) : base(output)
        {
            var config = new Config();
             reader = new ResponseComposer(config, s => alerts.Add(s));
        }

        [Fact]
        public void T03_String()
        {
            Assert.Throws<ArgumentNullException>(() => reader.Parse<string>(null));
            Assert.Equal("", reader.Parse<string>(""));
            Assert.Equal("string", reader.Parse<string>("string"));
            Assert.Empty(alerts);
        }

        [Fact]
        public void T04_Bool()
        {
            Assert.Throws<ArgumentNullException>(() => reader.Parse<bool>(null));
            Assert.Throws<InvalidDataException>(() => reader.Parse<bool>(""));
            Assert.Throws<InvalidDataException>(() => reader.Parse<bool?>(""));
            Assert.Equal(false, reader.Parse<bool>("faLse"));
            Assert.Equal(true, reader.Parse<bool>("TrUe"));
            Assert.Equal(false, reader.Parse<bool>("0"));
            Assert.Equal(true, reader.Parse<bool>("1"));
            Assert.Throws<InvalidDataException>(() => reader.Parse<bool>("2"));
            Assert.Throws<InvalidDataException>(() => reader.Parse<bool>("invalid"));
            Assert.Empty(alerts);
        }

        [Fact]
        public void T05_DateTime()
        {
            Assert.Throws<ArgumentNullException>(() => reader.Parse<LocalTime>(null));
            Assert.Throws<UnparsableValueException> (() => reader.Parse<LocalTime>(""));
            Assert.Throws<InvalidDataException>(() => reader.Parse<LocalTime?>("10:30")); // nullable Date not supported
            reader.Parse<LocalTime>("10:30");
            reader.Parse<LocalDateTime>("20201220 21:59:32");
        }

        [Fact]
        public void T06_Char()
        {
            Assert.Throws<ArgumentNullException>(() => reader.Parse<char>(null));
            Assert.Throws<InvalidDataException>(() => reader.Parse<char?>(""));
            Assert.Throws<InvalidDataException>(() => reader.Parse<char?>("A"));
            Assert.Throws<InvalidDataException>(() => reader.Parse<char>(""));
            Assert.Throws<InvalidDataException>(() => reader.Parse<char>("AB"));
            Assert.Equal('A', reader.Parse<char>("A"));
        }

        [Fact]
        public void T07_Number()
        {
            Assert.Throws<ArgumentNullException>(() => reader.Parse<int>(null));
            //Assert.Throws<InvalidDataException>(() => reader.Parse<int>(""));
            Assert.Equal(0, reader.Parse<int>(""));
            Assert.Throws<InvalidDataException>(() => reader.Parse<int>("invalid"));
            Assert.Equal(0, reader.Parse<int>("0"));
            Assert.Equal(123, reader.Parse<int>("123"));
            Assert.Equal(-123, reader.Parse<int>("-123"));
            Assert.Throws<InvalidDataException>(() => reader.Parse<int>("1.5"));
            Assert.Equal(123, reader.Parse<long>("123"));
            Assert.Equal(1.1, reader.Parse<double>("1.1"));
            Assert.Equal(null, reader.Parse<long?>(""));
            Assert.Equal(null, reader.Parse<double?>(""));
            Assert.Equal(null, reader.Parse<int?>(""));
            Assert.Equal(123, reader.Parse<int?>("123"));
            Assert.Equal(123, reader.Parse<long?>("123"));
            Assert.Equal(123, reader.Parse<double?>("123"));
            Assert.Empty(alerts);
        }

        public enum TestEnum { Two = 2 }

        [Fact]
        public void T08_Enum()
        {
            Assert.Throws<ArgumentNullException>(() => reader.Parse<TestEnum>(null));
            Assert.Throws<InvalidDataException>(() => reader.Parse<TestEnum>(""));
            Assert.Throws<InvalidDataException>(() => reader.Parse<TestEnum?>("2"));
            Assert.Equal(TestEnum.Two, reader.Parse<TestEnum>("2"));
            Assert.Throws<InvalidDataException>(() => reader.Parse<TestEnum>("Two"));
            Assert.Empty(alerts);
            Assert.Equal(99, (int)reader.Parse<TestEnum>("99")); // new value
            Assert.IsType<ResponseWarning>(alerts.Single());
        }

    }
}