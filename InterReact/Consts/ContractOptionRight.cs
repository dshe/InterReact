namespace InterReact;

public static class ContractOptionRight
{
    /// <summary>
    /// Contract is not an option.
    /// </summary>
    public static readonly string Undefined = ""; // "0", "", "?"

    /// <summary>
    /// Option type is PUT.
    /// </summary>
    public static readonly string Put = "P";

    /// <summary>
    /// Option type is a CALL.
    /// </summary>
    public static readonly string Call = "C";
}
