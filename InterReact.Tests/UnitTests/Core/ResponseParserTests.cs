using System;
using System.Linq;
using InterReact.Core;
using InterReact.Tests.Utility;
using Xunit;
using Xunit.Abstractions;
using System.Collections.Generic;
using NodaTime;
using NodaTime.Text;

namespace InterReact.Tests.UnitTests.Core
{
    public class ResponseParserTests : BaseUnitTest
    {
        private readonly List<object> alerts = new List<object>();
        private readonly ResponseParser parser;
        public ResponseParserTests(ITestOutputHelper output) : base(output) =>
            parser = new ResponseParser(s => alerts.Add(s));

        [Fact]
        public void T04_Bool()
        {
            //Assert.Throws<ArgumentNullException>(() => parser.ParseBool(null));
            Assert.False(parser.ParseBool(""));
            Assert.False(parser.ParseBool("faLse"));
            Assert.True(parser.ParseBool("TrUe"));
            Assert.False(parser.ParseBool("0"));
            Assert.True(parser.ParseBool("1"));
            Assert.True(parser.ParseBool("2"));
            Assert.Throws<ArgumentException>(() => parser.ParseBool("invalid"));
            //Assert.Throws<ArgumentException>(() => parser.ParseBool("true"));
            Assert.Empty(alerts);
        }

        [Fact]
        public void T06_Char()
        {
            //Assert.Throws<ArgumentNullException>(() => parser.ParseChar(null));
            Assert.Equal('\0', parser.ParseChar(""));
            //Assert.Throws<ArgumentException>(() => parser.ParseChar("A"));
            Assert.Throws<ArgumentException>(() => parser.ParseChar("AB"));
            Assert.Equal('A', parser.ParseChar("A"));
        }

        [Fact]
        public void T07_Number()
        {
            //Assert.Throws<ArgumentNullException>(() => parser.ParseInt(null));
            Assert.Equal(0, parser.ParseInt(""));
            Assert.Equal(0, parser.ParseInt("0"));
            Assert.Throws<ArgumentException>(() => parser.ParseInt("invalid"));
            Assert.Equal(123, parser.ParseInt("123"));
            Assert.Equal(-123, parser.ParseInt("-123"));
            Assert.Throws<ArgumentException>(() => parser.ParseInt("1.5"));
            Assert.Equal(123, parser.ParseLong("123"));
            Assert.Equal(42.1, parser.ParseDouble("42.1"));
            Assert.Equal(123, parser.ParseIntNullable("123"));
            Assert.Null(parser.ParseIntNullable(""));
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
            Assert.IsType<ResponseWarning>(alerts.Single());
        }
    }
}