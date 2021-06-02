using System;
using System.Collections.Generic;
using Xunit;
using Xunit.Abstractions;

namespace InterReact.UnitTests.Core
{
    public class Response_Parser_Tests : BaseUnitTest
    {
        private readonly List<object> alerts = new List<object>();
        private readonly ResponseParser parser;
        public Response_Parser_Tests(ITestOutputHelper output) : base(output) =>
            parser = new ResponseParser(Logger);

        [Fact]
        public void T04_Bool()
        {
            //Assert.Throws<ArgumentNullException>(() => parser.ParseBool(null));
            Assert.False(ResponseParser.ParseBool(""));
            Assert.False(ResponseParser.ParseBool("faLse"));
            Assert.True(ResponseParser.ParseBool("TrUe"));
            Assert.False(ResponseParser.ParseBool("0"));
            Assert.True(ResponseParser.ParseBool("1"));
            Assert.True(ResponseParser.ParseBool("2"));
            Assert.Throws<ArgumentException>(() => ResponseParser.ParseBool("invalid"));
            //Assert.Throws<ArgumentException>(() => parser.ParseBool("true"));
            Assert.Empty(alerts);
        }

        [Fact]
        public void T06_Char()
        {
            //Assert.Throws<ArgumentNullException>(() => parser.ParseChar(null));
            Assert.Equal('\0', ResponseParser.ParseChar(""));
            //Assert.Throws<ArgumentException>(() => parser.ParseChar("A"));
            Assert.Throws<ArgumentException>(() => ResponseParser.ParseChar("AB"));
            Assert.Equal('A', ResponseParser.ParseChar("A"));
        }

        [Fact]
        public void T07_Number()
        {
            //Assert.Throws<ArgumentNullException>(() => parser.ParseInt(null));
            Assert.Equal(0, ResponseParser.ParseInt(""));
            Assert.Equal(0, ResponseParser.ParseInt("0"));
            Assert.Throws<ArgumentException>(() => ResponseParser.ParseInt("invalid"));
            Assert.Equal(123, ResponseParser.ParseInt("123"));
            Assert.Equal(-123, ResponseParser.ParseInt("-123"));
            Assert.Throws<ArgumentException>(() => ResponseParser.ParseInt("1.5"));
            Assert.Equal(123, ResponseParser.ParseLong("123"));
            Assert.Equal(42.1, ResponseParser.ParseDouble("42.1"));
            Assert.Equal(123, ResponseParser.ParseIntNullable("123"));
            Assert.Null(ResponseParser.ParseIntNullable(""));
            Assert.Empty(alerts);
        }

        public enum TestEnum { Two = 2 }

        [Fact]
        public void T08_Enum()
        {
            //Assert.Throws<ArgumentNullException>(() => parser.ParseEnum<TestEnum>(null));
            Assert.Throws<ArgumentException>(() => parser.ParseEnum<TestEnum>(""));
            //Assert.Throws<ArgumentException>(() => parser.ParseEnum<TestEnum?>("2"));
            Assert.Equal(TestEnum.Two, parser.ParseEnum<TestEnum>("2"));
            Assert.Throws<ArgumentException>(() => parser.ParseEnum<TestEnum>("Two"));
            Assert.Empty(alerts);
            Assert.Equal(99, (int)parser.ParseEnum<TestEnum>("99")); // new value
            //Assert.IsType<ResponseWarning>(alerts.Single());
        }
    }
}