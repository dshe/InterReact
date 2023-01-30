namespace InterReact;

public sealed class FinancialAdvisor
{
    public FinancialAdvisorDataType DataType { get; } = FinancialAdvisorDataType.Undefined;
    public string Data { get; } = "";
    internal FinancialAdvisor(ResponseReader r)
    {
        r.IgnoreMessageVersion();
        DataType = r.ReadEnum<FinancialAdvisorDataType>();
        Data = r.ReadString();
    }
}
