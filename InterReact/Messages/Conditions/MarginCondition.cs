namespace InterReact
{
    /**
    * @class MarginCondition
    * @brief This class represents a condition requiring the margin cushion reaching a given percent to be fulfilled.
    * Orders can be activated or canceled if a set of given conditions is met. A MarginCondition is met whenever the margin penetrates the given percent.
    */
    public class MarginCondition : OperatorCondition
    {
        const string header = "the margin cushion percent";

        protected override string Value
        {
            get => Percent.ToString();
            set => Percent = int.Parse(value);
        }

        public override string ToString() => header + base.ToString();

        public int Percent { get; set; }

        protected override bool TryParse(string cond)
        {
            if (!cond.StartsWith(header))
                return false;
            cond = cond.Replace(header, "");
            return base.TryParse(cond);
        }
    }
}
