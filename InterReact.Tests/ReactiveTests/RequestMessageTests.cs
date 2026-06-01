using Microsoft.Extensions.Logging.Abstractions;
using System.Globalization;
namespace UnitTests;

public class RequestMessageTests(ITestOutputHelper output) : OutputHelperTestBase(output)
{
    private readonly RequestMessage _requestMessage =
        new(NullLogger<RequestMessage>.Instance, Connection.NullInstance);

    private void AssertResult(params string[] strings)
    {
        List<string> messageStrings = _requestMessage.Strings;
        Assert.Equal(messageStrings.Count, strings.Length);
        for (int i = 0; i < messageStrings.Count; i++)
            Assert.Equal(messageStrings[i], strings[i]);
        messageStrings.Clear();
    }

    [Fact]
    public void T01_Null()
    {
        _requestMessage.Write(null);
        AssertResult("");
    }

    [Fact]
    public void T02_Empty()
    {
        _requestMessage.Write("");
        AssertResult("");
    }

    [Fact]
    public void T03_Nulls()
    {
        _requestMessage.Write(null).Write(null);
        AssertResult("", "");
    }

    [Fact]
    public void T04_String()
    {
        _requestMessage.Write("test");
        AssertResult("test");
    }

    [Fact]
    public void T05_Strings()
    {
        _requestMessage.Write("test1").Write("test2");
        AssertResult("test1", "test2");
    }

    [Fact]
    public void T06_Int()
    {
        _requestMessage.Write(42);
        AssertResult("42");
    }

    [Fact]
    public void T08_Contract()
    {
        var contract = new Contract();
        Assert.ThrowsAny<Exception>(() => _requestMessage.Write(1, "str", contract));
    }

    [Fact]
    public void T08_EnumsToNumbers()
    {
        DayOfWeek[] _ = [DayOfWeek.Saturday, DayOfWeek.Sunday];

        var x2 = GetEnumValuesString(Array.Empty<DayOfWeek>());
        Write(x2);
    }

    private static string GetEnumValuesString<T>(IEnumerable<T>? enums) where T : Enum
    {
        if (enums is null)
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
