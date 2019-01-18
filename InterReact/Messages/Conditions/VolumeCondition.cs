namespace InterReact.Messages.Conditions
{
    /**
    * @brief Used with conditional orders to submit or cancel an order based on a specified volume change in a security. 
    */
    public class VolumeCondition : ContractCondition
    {
        protected override string Value
        {
            get
            {
                return Volume.ToString();
            }
            set
            {
                Volume = int.Parse(value);
            }
        }

        public int Volume { get; set; }
    }
}
