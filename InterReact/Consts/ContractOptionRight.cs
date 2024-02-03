namespace InterReact;

public static class ContractOptionRight
{
    /// <summary>
    /// Contract is not an option.
    /// </summary>
    public const string Undefined = ""; // "0", "", "?"

    /// <summary>
    /// Option type is PUT.
    /// </summary>
    public const string Put = "P";

    /// <summary>
    /// Option type is a CALL.
    /// </summary>
    public const string Call = "C";
}
