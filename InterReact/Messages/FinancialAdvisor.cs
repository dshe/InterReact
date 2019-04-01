using InterReact.Core;
using InterReact.Enums;

namespace InterReact.Messages
{
    public sealed class FinancialAdvisor // output
    {
        public FinancialAdvisorDataType DataType { get; }
        public string XmlData { get; }
        internal FinancialAdvisor(ResponseComposer c)
        {
            c.IgnoreVersion();
            DataType = c.ReadEnum<FinancialAdvisorDataType>();
            XmlData = c.ReadString();
        }
    }
}
