namespace InterReact
{
    public sealed class FinancialAdvisor // output
    {
        public FinancialAdvisorDataType DataType { get; }
        public string XmlData { get; }
        internal FinancialAdvisor(ResponseReader r)
        {
            r.IgnoreVersion();
            DataType = r.ReadEnum<FinancialAdvisorDataType>();
            XmlData = r.ReadString();
        }
    }
}
