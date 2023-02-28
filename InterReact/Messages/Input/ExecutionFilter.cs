namespace InterReact;

public sealed class ExecutionFilter // input
{
     /// <summary>
    /// Zero means no filtering on this field.
    /// </summary>
    public int ClientId { get; set; }
    /// <summary>
    /// This is only relevant for Financial Advisor (FA) accounts.
    /// </summary>
    public string Account { get; set; } = "";
    public string Time { get; set; } = "";
    public string Symbol { get; set; } = "";
    public SecurityType SecurityType { get; set; } = SecurityType.Undefined;
    public string Exchange { get; set; } = "";
    public string Side { get; set; } = "";
}
