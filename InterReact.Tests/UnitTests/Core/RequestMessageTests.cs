using Microsoft.Extensions.Logging.Abstractions;
using RxSockets;
using Stringification;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Core;

public class RequestMessageTests : UnitTestBase
{
    private readonly RequestMessage requestMessage = 
        new(NullLoggerFactory.Instance, NullRxSocketClient.Instance, new RingLimiter());
    public RequestMessageTests(ITestOutputHelper output) : base(output) {}

    private void AssertResult(params string[] strings)
    {
        List<string> messageStrings = requestMessage.Strings;
        Assert.Equal(messageStrings.Count, strings.Length);
        for (int i = 0; i < messageStrings.Count; i++)
            Assert.Equal(messageStrings[i], strings[i]);
        messageStrings.Clear();
    }

    [Fact]
    public void T01_Null()
    {
        requestMessage.Write(null);
        AssertResult("");
    }

    [Fact]
    public void T02_Empty()
    {
        requestMessage.Write("");
        AssertResult("");
    }

    [Fact]
    public void T03_Nulls()
    {
        requestMessage.Write(null).Write(null);
        AssertResult("", "");
    }

    [Fact]
    public void T04_String()
    {
        requestMessage.Write("test");
        AssertResult("test");
    }

    [Fact]
    public void T05_Strings()
    {
        requestMessage.Write("test1").Write("test2");
        AssertResult("test1", "test2");
    }

    [Fact]
    public void T06_Int()
    {
        requestMessage.Write(42);
        AssertResult("42");
    }

    [Fact]
    public void T08_Contract()
    {
        var contract = new Contract();
        Assert.ThrowsAny<Exception>(() => requestMessage.Write(1, "str", contract));
    }

    [Fact]
    public void T08_EnumsToNumbers()
    {
        DayOfWeek[] enums = new DayOfWeek[] { DayOfWeek.Saturday, DayOfWeek.Sunday };

        var x2 = GetEnumValuesString(new DayOfWeek[] {});
        Write(x2);

    }

    private string GetEnumValuesString<T>(IEnumerable<T>? enums) where T : Enum
    {
        if (enums == null)
            return "";

        List<string> list = enums.Select(enm =>
            GetEnumValueString(enm))
            .ToList();

        return string.Join(",", list);

        // local
        static string GetEnumValueString(T en)
        {
            Type underlyingType = Enum.GetUnderlyingType(typeof(T));
            object o = Convert.ChangeType(en, underlyingType, CultureInfo.InvariantCulture) 
                ?? throw new ArgumentException("Could not get enum value.");
            return o.ToString() ?? throw new ArgumentException("Could not get enum value.");
        }
    }



}
