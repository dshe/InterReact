using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace InterReact.UnitTests.Core;

public class ResponseComposerTests : UnitTestsBase
{
    internal ResponseComposer Composer;
    internal List<object> Messages = new();
    internal void Send(object message) => Messages.Add(message);

    public ResponseComposerTests(ITestOutputHelper output) : base(output) =>
        Composer = new ResponseComposer(new Config(), Send);

    [Fact]
    public void T00_Null()
    {
        Assert.Throws<ArgumentNullException>(() => Composer.ComposeAndSend(null!));
    }

    [Fact]
    public void T01_Short_Message()
    {
        var e = Assert.Throws<InvalidDataException>(() => Composer.ComposeAndSend(Array.Empty<string>()));
        Assert.IsType<InvalidDataException>(e.InnerException);

        e = Assert.Throws<InvalidDataException>(() => Composer.ComposeAndSend(new[] { "2", "2" })); // short
        Assert.IsType<InvalidDataException>(e.InnerException);
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
        Assert.Throws<InvalidDataException>(() => Composer.ComposeAndSend(longMessage));
    }

    [Fact]
    public void T03_Undefined_Code()
    {
        var e = Assert.Throws<InvalidDataException>(() => Composer.ComposeAndSend(new string[] { "" }));
        Assert.Equal("Undefined code ''.", e.InnerException?.Message);

        e = Assert.Throws<InvalidDataException>(() => Composer.ComposeAndSend(new string[] { "notInSwitch" }));
        Assert.Equal("Undefined code 'notInSwitch'.", e.InnerException?.Message);
    }

    [Fact]
    public void T04_ParseError()
    {
        var e = Assert.Throws<InvalidDataException>(() => Composer.ComposeAndSend(new[] { "1", "version", "cannotParseInt" }));
        Assert.IsType<ArgumentException>(e.InnerException);
        Assert.StartsWith("Parse", e.InnerException?.Message);
    }

    [Fact]
    public void T03_Ok()
    {
        Composer.ComposeAndSend(new[] {
                "2",  // code = size
                "99", // version
                "2",  // requestId
                "5",  // tickType
                "100" // size
            });

        object message = Messages.Single();
        Assert.IsType<SizeTick>(message);
    }
}
