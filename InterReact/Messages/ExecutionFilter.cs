using NodaTime;

namespace InterReact;

public sealed class ExecutionFilter // input
{
    /// <summary>
    /// This is only relevant for Financial Advisor (FA) accounts.
    /// </summary>
    public string Account { get; init; } = "";

    /// <summary>
    /// Zero means no filtering on this field.
    /// </summary>
    public int ClientId { get; init; }
    public LocalDateTime Time { get; init; }
    public string Symbol { get; init; } = "";
    public SecurityType SecurityType { get; init; } = SecurityType.Undefined;
    public string Exchange { get; init; } = "";
    public string Side { get; init; } = "";
}
