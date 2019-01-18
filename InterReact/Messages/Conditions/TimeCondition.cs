namespace InterReact.Messages.Conditions
{
    /**
    * @brief Time condition used in conditional orders to submit or cancel orders at specified time. 
    */
    public class TimeCondition : OperatorCondition
    {
        const string header = "time";

        protected override string Value
        {
            get
            {
                return Time;
            }
            set
            {
                Time = value;
            }
        }

        public override string ToString()
        {
            return header + base.ToString();
        }

        /**
        * @brief Time field used in conditional order logic. Valid format: YYYYMMDD HH:MM:SS
        */

        public string Time { get; set; }

        protected override bool TryParse(string cond)
        {
            if (!cond.StartsWith(header))
                return false;

            cond = cond.Replace(header, "");
            return base.TryParse(cond);
        }
    }
}
