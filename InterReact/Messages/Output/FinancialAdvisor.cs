namespace InterReact;

public sealed class FinancialAdvisor // output
{
    public FinancialAdvisorDataType DataType { get; } = FinancialAdvisorDataType.Undefined;
    public string Data { get; } = "";
    internal FinancialAdvisor() { }
    internal FinancialAdvisor(ResponseReader r)
    {
        r.IgnoreMessageVersion();
        DataType = r.ReadEnum<FinancialAdvisorDataType>();
        Data = r.ReadString();
    }
}
