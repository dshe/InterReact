using Microsoft.Extensions.Logging.Abstractions;

namespace Core;

public class ResponseComposerTests : UnitTestBase
{
    private readonly ResponseMessageComposer Composer;

    public ResponseComposerTests(ITestOutputHelper output) : base(output)
    {
        //var connection = new Connection();
        var connector = new InterReactClientConnector();
        var connection = new Connection(connector);
        Composer = new ResponseMessageComposer(connection, NullLoggerFactory.Instance);
    }

    [Fact]
    public void T00_Null()
    {
        Assert.Throws<ArgumentNullException>(() => Composer.ComposeMessage(null!));
    }

    [Fact]
    public void T01_Short_Message()
    {
        InvalidDataException e = Assert.Throws<InvalidDataException>(() => Composer.ComposeMessage(Array.Empty<string>()));
        Assert.IsType<InvalidDataException>(e.InnerException);

        e = Assert.Throws<InvalidDataException>(() => Composer.ComposeMessage(new[] { "2", "2" })); // short
        Assert.IsType<InvalidDataException>(e.InnerException);
    }

    [Fact]
    public void T02_Long_Message()
    {
        string[] longMessage = new[] {
                "1",  // code = TickPrice
                "99", // version
                "21",  // requestId
                "5",  // tickType = LastSize
                "9.9", // price
                "100", // size
                "1", // AutoExec, true
                "too long", // extra, to make it too long
            };
        Assert.Throws<InvalidDataException>(() => Composer.ComposeMessage(longMessage));
    }

    [Fact]
    public void T03_Undefined_Code()
    {
        InvalidDataException e = Assert.Throws<InvalidDataException>(() => Composer.ComposeMessage(new string[] { "" }));
        Assert.Equal("Undefined code ''.", e.InnerException?.Message);

        e = Assert.Throws<InvalidDataException>(() => Composer.ComposeMessage(new string[] { "notInSwitch" }));
        Assert.Equal("Undefined code 'notInSwitch'.", e.InnerException?.Message);
    }

    [Fact]
    public void T04_ParseError()
    {
        InvalidDataException e = Assert.Throws<InvalidDataException>(() => Composer.ComposeMessage(new[] { "1", "version", "cannotParseInt" }));
        Assert.IsType<ArgumentException>(e.InnerException);
        Assert.StartsWith("Parse", e.InnerException?.Message);
    }

    [Fact]
    public void T03_Ok()
    {
        object message = Composer.ComposeMessage(new[] {
                "2",  // code = size
                "99", // version
                "2",  // requestId
                "5",  // tickType
                "100" // size
            });

        Assert.IsType<SizeTick>(message);
    }
}
