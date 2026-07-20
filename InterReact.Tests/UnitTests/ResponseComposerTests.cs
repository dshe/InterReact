using Microsoft.Extensions.Logging.Abstractions;
namespace UnitTests;

public class ResponseComposerTests : OutputHelperTestBase
{
    private readonly ResponseComposer _composer;
    public ResponseComposerTests(ITestOutputHelper output) : base(output)
    {
        ResponseParser parser = new(NullLogger<ResponseParser>.Instance);
        ResponseReader reader = new(parser, new(), NullLogger<ResponseReader>.Instance);
        _composer = new ResponseComposer(reader);
    }

    [Fact]
    public void T00_Null()
    {
        Assert.Throws<NullReferenceException>(() => _composer.Compose(null!));
    }

    [Fact]
    public void T01_Short_Message()
    {
        Assert.Throws<InvalidDataException>(() => _composer.Compose([]));
        Assert.Throws<InvalidDataException>(() => _composer.Compose(["2", "2"])); // short
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
        Assert.Throws<InvalidDataException>(() => _composer.Compose(longMessage));
    }

    [Fact]
    public void T03_Undefined_Code()
    {
        InvalidDataException e = Assert.Throws<InvalidDataException>(() => _composer.Compose([""]));
        Assert.Equal("Undefined code ''.", e.Message);

        e = Assert.Throws<InvalidDataException>(() => _composer.Compose(["notInSwitch"]));
        Assert.Equal("Undefined code 'notInSwitch'.", e.Message);
    }

    [Fact]
    public void T04_ParseError()
    {
        //ArgumentException e = Assert.Throws<ArgumentException>(() => _composer.Compose(["1", "version", "cannotParseInt"]));
        var e = Assert.Throws<InvalidDataException>(() => _composer.Compose(["2", "version", "cannotParse"]));
        Assert.StartsWith("ParseInt('cannotParse') failure.", e.InnerException?.Message);
        //Write(e.Message);
        Logger.LogError(e, "");
    }

    [Fact]
    public void T03_Ok()
    {
        object message = _composer.Compose([
            "2",  // code = size
            "99", // version
            "2",  // requestId
            "5",  // tickType
            "100" // size
            ]);

        Assert.IsType<SizeTick>(message);
    }
}
