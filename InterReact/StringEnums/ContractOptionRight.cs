namespace InterReact;

public sealed record ContractOptionRight(string StringCode) : IHasStringCode
{
    public static readonly ContractOptionRight Undefined = new(""); // "0", "", "?"
    public static readonly ContractOptionRight Put = new("P");
    public static readonly ContractOptionRight Call = new("C");
}
