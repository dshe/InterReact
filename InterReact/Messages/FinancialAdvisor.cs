using InterReact.Enums;

namespace InterReact.Messages
{
    public sealed class FinancialAdvisor // output
    {
        public FinancialAdvisorDataType DataType { get; internal set; }

        public string XmlData { get; internal set; }
    }
}
