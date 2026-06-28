namespace InterReact;

public sealed record OrderHedgeType(string Code) : IHasCode
{
    public static readonly OrderHedgeType Undefined = new("");
    public static readonly OrderHedgeType Delta = new("D");
    public static readonly OrderHedgeType Beta = new("B");
    public static readonly OrderHedgeType Forex = new("F");
    public static readonly OrderHedgeType Pair = new("P");
}
