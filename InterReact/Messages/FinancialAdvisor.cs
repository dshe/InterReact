namespace InterReact
{
    public sealed class FinancialAdvisor // output
    {
        public FinancialAdvisorDataType DataType { get; }
        public string XmlData { get; }
        internal FinancialAdvisor(ResponseReader c)
        {
            c.IgnoreVersion();
            DataType = c.ReadEnum<FinancialAdvisorDataType>();
            XmlData = c.ReadString();
        }
    }
}
