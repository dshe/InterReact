namespace InterReact;

public sealed class ExecutionFilter // input
{
    /// <summary>
    /// Zero means no filtering on this field.
    /// </summary>
    public int ClientId { get; init; }
    /// <summary>
    /// This is only relevant for Financial Advisor (FA) accounts.
    /// </summary>
    public string Account { get; init; } = "";
    public string Time { get; init; } = "";
    public string Symbol { get; init; } = "";
    public string SecurityType { get; init; } = "";
    public string Exchange { get; init; } = "";
    public string Side { get; init; } = "";
}
