using Microsoft.Extensions.Logging.Abstractions;
namespace UnitTests;

public class ResponseComposerTests : OutputHelperTestBase
{
    private readonly ResponseMessageComposer _composer;

    public ResponseComposerTests(ITestOutputHelper output) : base(output)
    {
        ResponseParser parser = new(NullLogger<ResponseParser>.Instance);
        ResponseReader reader = new(new(null), parser, NullLogger<ResponseReader>.Instance);
        _composer = new ResponseMessageComposer(SystemClock.Instance, reader);
    }

    [Fact]
    public void T00_Null()
    {
        Assert.Throws<NullReferenceException>(() => _composer.ComposeMessage(null!));
    }

    [Fact]
    public void T01_Short_Message()
    {
        Assert.Throws<InvalidDataException>(() => _composer.ComposeMessage([]));
        Assert.Throws<InvalidDataException>(() => _composer.ComposeMessage(["2", "2"])); // short
    }

    [Fact]
    public void T02_Long_Message()
    {
        string[] longMessage = [
                "1",  // code = TickPrice
                "99", // version
                "21",  // requestId
                "5",  // tickType = LastSize
                "9.9", // price
                "100", // size
                "1", // AutoExec, true
                "too long", // extra, to make it too long
            ];
        Assert.Throws<InvalidDataException>(() => _composer.ComposeMessage(longMessage));
    }

    [Fact]
    public void T03_Undefined_Code()
    {
        InvalidDataException e = Assert.Throws<InvalidDataException>(() => _composer.ComposeMessage([""]));
        Assert.StartsWith("Error occurred in ResponseMessageCompose", e.Message);
        Assert.Equal("Undefined code ''.", e.InnerException?.Message);

        e = Assert.Throws<InvalidDataException>(() => _composer.ComposeMessage(["notInSwitch"]));
        Assert.StartsWith("Error occurred in ResponseMessageCompose", e.Message);
        Assert.Equal("Undefined code 'notInSwitch'.", e.InnerException?.Message);
        ;
    }

    [Fact]
    public void T04_ParseError()
    {
        InvalidDataException e = Assert.Throws<InvalidDataException>(() => _composer.ComposeMessage(["1", "version", "cannotParseInt"]));
        Assert.StartsWith("Error occurred in ResponseMessageCompose", e.Message);
    }

    [Fact]
    public void T03_Ok()
    {
        object message = _composer.ComposeMessage([
                "2",  // code = size
                "99", // version
                "2",  // requestId
                "5",  // tickType
                "100" // size
            ]);

        Assert.IsType<SizeTick>(message);
    }
}
