using Microsoft.Extensions.Logging.Abstractions;
namespace UnitTests;

public class ResponseParserTests(ITestOutputHelper output) : OutputHelperTestBase(output)
{
    private readonly List<object> _alerts = [];
    private readonly ResponseParser _parser = new(NullLogger<ResponseParser>.Instance);

    [Fact]
    public void T04_Bool()
    {
        //Assert.Throws<ArgumentNullException>(() => parser.ParseBool(null));
        Assert.False(_parser.ParseBool(""));
        Assert.False(_parser.ParseBool("faLse"));
        Assert.True(_parser.ParseBool("TrUe"));
        Assert.False(_parser.ParseBool("0"));
        Assert.True(_parser.ParseBool("1"));
        Assert.True(_parser.ParseBool("2"));
        Assert.Throws<ArgumentException>(() => _parser.ParseBool("invalid"));
        Assert.Empty(_alerts);
    }

    [Fact]
    public void T06_Char()
    {
        //Assert.Throws<ArgumentNullException>(() => parser.ParseChar(null));
        Assert.Equal('\0', _parser.ParseChar(""));
        //Assert.Throws<ArgumentException>(() => parser.ParseChar("A"));
        Assert.Throws<ArgumentException>(() => _parser.ParseChar("AB"));
        Assert.Equal('A', _parser.ParseChar("A"));
    }

    [Fact]
    public void T07_Number()
    {
        //Assert.Throws<ArgumentNullException>(() => parser.ParseInt(null));
        Assert.Equal(0, _parser.ParseInt(""));
        Assert.Equal(0, _parser.ParseInt("0"));
        Assert.Throws<ArgumentException>(() => _parser.ParseInt("invalid"));
        Assert.Equal(123, _parser.ParseInt("123"));
        Assert.Equal(-123, _parser.ParseInt("-123"));
        Assert.Throws<ArgumentException>(() => _parser.ParseInt("1.5"));
        Assert.Equal(123, _parser.ParseLong("123"));
        Assert.Equal(42.1, _parser.ParseDouble("42.1"));
        Assert.Empty(_alerts);
    }

    private enum TestEnum { Two = 2 }

    [Fact]
    public void T08_Enum()
    {
        //Assert.Throws<ArgumentNullException>(() => parser.ParseEnum<TestEnum>(null));
        Assert.Throws<ArgumentException>(() => _parser.ParseEnum<TestEnum>(""));
        //Assert.Throws<ArgumentException>(() => parser.ParseEnum<TestEnum?>("2"));
        Assert.Equal(TestEnum.Two, _parser.ParseEnum<TestEnum>("2"));
        Assert.Throws<ArgumentException>(() => _parser.ParseEnum<TestEnum>("Two"));
        Assert.Empty(_alerts);
        Assert.Equal(99, (int)_parser.ParseEnum<TestEnum>("99")); // new value
        //Assert.IsType<ResponseWarning>(alerts.Single());
    }
}
