namespace InterReact;

[Message]
public sealed record FinancialAdvisor
{
    public FinancialAdvisorDataType DataType { get; init; }
    public string Data { get; init; } = "";
    internal FinancialAdvisor() { }
    internal FinancialAdvisor(ResponseReader r)
    {
        r.IgnoreMessageVersion();
        DataType = r.ReadEnum<FinancialAdvisorDataType>();
        Data = r.ReadString();
    }
}
