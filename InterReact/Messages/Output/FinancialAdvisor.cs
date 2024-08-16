namespace InterReact;

public sealed class FinancialAdvisor
{
    public FinancialAdvisorDataType DataType { get; }
    public string Data { get; }
    internal FinancialAdvisor(ResponseReader r)
    {
        r.IgnoreMessageVersion();
        DataType = r.ReadEnum<FinancialAdvisorDataType>();
        Data = r.ReadString();
    }
}
