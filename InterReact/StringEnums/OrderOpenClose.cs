namespace InterReact;

public sealed record OrderOpenClose(string Code) : IHasCode
{
    public static readonly OrderOpenClose Undefined = new("");
    public static readonly OrderOpenClose Open = new("O");
    /// <summary>
    /// Close. Institutional orders only.
    /// </summary>
    public static readonly OrderOpenClose Close = new("C");
}
