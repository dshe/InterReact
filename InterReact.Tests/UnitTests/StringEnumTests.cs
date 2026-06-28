namespace UnitTests;

public class EnumTests(ITestOutputHelper output) : OutputHelperTestBase(output)
{
    [Fact]
    public void T01_ParseEnum()
    {
        ResponseParser parser = new(LogFactory.CreateLogger<ResponseParser>());

        VolatilityType x = parser.ParseEnum<VolatilityType>("1");
        Write(x.ToString());

        VolatilityType y = parser.ParseEnum<VolatilityType>("99");
        Write(y.ToString());

        Assert.ThrowsAny<Exception>(() => parser.ParseEnum<VolatilityType>("something"));
    }

    [Fact]
    public void T01_ParseStringEnum()
    {
        ResponseParser parser = new(LogFactory.CreateLogger<ResponseParser>());

        ContractOptionRight x = parser.ParseStringEnum<ContractOptionRight>("P");
        Write(x.Code);

        ContractOptionRight y = parser.ParseStringEnum<ContractOptionRight>("p");
        Write(y.Code);

        ContractOptionRight z = parser.ParseStringEnum<ContractOptionRight>("Zz");
        Write(z.Code);
    }
}

